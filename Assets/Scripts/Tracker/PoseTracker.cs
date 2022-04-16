// Copyright 2021-2022 The SeedV Lab.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;
using System.Collections.Generic;
using Logger = Mediapipe.Unity.Logger;  // Disambiguation with UnityEngine.Logger
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.Holistic;
using UnityEngine;

[RequireComponent(typeof(HolisticTrackingGraph))]
public class PoseTracker : MonoBehaviour {
  private const string _TAG = nameof(PoseTracker);
  public enum InferenceMode {
    CPU,
    GPU,
  }
  public RunningMode runningMode;
  [SerializeField] private InferenceMode _preferableInferenceMode;
  [SerializeField] private TextureFramePool _textureFramePool;
  [SerializeField] private Texture _sourceTexture;
  [SerializeField] private PoseLandmarkListAnnotationController _annotationController;
  [SerializeField] private bool _hFlip;
  [SerializeField] private bool _vFlip;
  [SerializeField] private int _rotation;
  [SerializeField] private ModelAnimator _modelAnimator = null;
  private HolisticTrackingGraph _graphRunner;
  private Coroutine _coroutine;
  private InferenceMode _inferenceMode;

  public IEnumerator Start() {
    _graphRunner = GetComponent<HolisticTrackingGraph>();
    AssetLoader.Provide(new StreamingAssetsResourceManager());

    Logger.LogInfo(_TAG, "Starting mediapipe");
    DecideInferenceMode();
    if (_inferenceMode == InferenceMode.GPU) {
      Logger.LogInfo(_TAG, "Initializing GPU resources...");
      yield return GpuManager.Initialize();
    }
    var graphInitRequest = _graphRunner.WaitForInit(runningMode);
    _textureFramePool.ResizeTexture(_sourceTexture.width, _sourceTexture.height, TextureFormat.RGBA32);
    yield return graphInitRequest;
    if (graphInitRequest.isError) {
      Debug.Log(graphInitRequest.error);
      yield break;
    }
    OnStartRun();
    Logger.LogInfo(_TAG, "Graph Runner Init!");
    SidePacket sidePacket = _graphRunner.BuildSidePacket(_rotation, _hFlip, _vFlip);
    _graphRunner.StartRun(sidePacket);
    Logger.LogInfo(_TAG, "Graph Runner started in async mode!");

    _modelAnimator.Width = _sourceTexture.width;
    _modelAnimator.Height = _sourceTexture.height;
    _coroutine = StartCoroutine(ProcessImage(_sourceTexture));
  }

  public void OnDestroy() {
    if (_coroutine != null) StopCoroutine(_coroutine);
  }

  private IEnumerator ProcessImage(Texture image) {
    Logger.LogVerbose(_TAG, "Process image");
    while (true) {
      if (!_textureFramePool.TryGetTextureFrame(out var textureFrame)) {
        yield return new WaitForEndOfFrame();
        continue;
      }
      textureFrame.ReadTextureFromOnCPU(image);
      _graphRunner.AddTextureFrameToInputStream(textureFrame);
      yield return new WaitForEndOfFrame();
    }
  }

  private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs) {
    NormalizedLandmarkList poseLandmarks = eventArgs.value;
    if (_annotationController != null && poseLandmarks != null) {
      _annotationController.DrawLater(poseLandmarks);
    }
    if (_modelAnimator != null && poseLandmarks != null) {
      _modelAnimator.SetPoseLandmarks(poseLandmarks);
    }
  }

  private void DecideInferenceMode() {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
      if (_preferableInferenceMode == InferenceMode.GPU) {
        Debug.Log("Current platform does not support GPU inference mode, so falling back to CPU mode");
      }
      _inferenceMode = InferenceMode.CPU;
#else
    _inferenceMode = _preferableInferenceMode;
#endif
  }

  public void OnStartRun() {
    if (!runningMode.IsSynchronous()) {
      _graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
    }
  }  
}
