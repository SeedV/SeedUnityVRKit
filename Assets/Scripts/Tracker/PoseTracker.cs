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
using Logger = Mediapipe.Logger;  // Disambiguation with UnityEngine.Logger
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.PoseTracking;
using UnityEngine;

[RequireComponent(typeof(PoseTrackingGraph))]
public class PoseTracker : MonoBehaviour {
  private const string _TAG = nameof(PoseTracker);
  public enum InferenceMode {
    CPU,
    GPU
  }
  [SerializeField] private InferenceMode _preferableInferenceMode;
  [SerializeField] private TextureFramePool _textureFramePool;
  [SerializeField] private Texture _sourceTexture;
  [SerializeField] private PoseLandmarkListAnnotationController _annotationController;
  [SerializeField] private bool _hFlip;
  [SerializeField] private bool _vFlip;
  [SerializeField] private int _rotation;
  private PoseTrackingGraph _graphRunner;
  private Coroutine _coroutine;
  private InferenceMode _inferenceMode;

  public IEnumerator Start() {
    _graphRunner = GetComponent<PoseTrackingGraph>();
    AssetLoader.Provide(new StreamingAssetsResourceManager());

    Logger.LogInfo(_TAG, "Starting mediapipe");
    DecideInferenceMode();
    if (_inferenceMode == InferenceMode.GPU) {
      Logger.LogInfo(_TAG, "Initializing GPU resources...");
      yield return GpuManager.Initialize();
    }
    var graphInitRequest = _graphRunner.WaitForInit();
    yield return graphInitRequest;
    if (graphInitRequest.isError) {
      Debug.Log(graphInitRequest.error);
      yield break;
    }
    Logger.LogInfo(_TAG, "Graph Runner Init!");
    _graphRunner.OnPoseLandmarksOutput.AddListener(OnPoseLandmarksOutput);
    SidePacket sidePacket = new SidePacket();
    sidePacket.Emplace("input_horizontally_flipped", new BoolPacket(_hFlip));
    sidePacket.Emplace("input_vertically_flipped", new BoolPacket(_vFlip));
    sidePacket.Emplace("input_rotation", new IntPacket(_rotation));
    _graphRunner.StartRunAsync(sidePacket).AssertOk();
    Logger.LogInfo(_TAG, "Graph Runner started in async mode!");

    _textureFramePool.ResizeTexture(_sourceTexture.width, _sourceTexture.height, TextureFormat.RGBA32);
    _coroutine = StartCoroutine(ProcessImage(_sourceTexture));
  }

  public void OnDestroy() {
    if (_coroutine != null) StopCoroutine(_coroutine);
  }

  private IEnumerator ProcessImage(Texture image) {
    Logger.LogVerbose(_TAG, "Process image");
    while (true) {
      var textureFrameRequest = _textureFramePool.WaitForNextTextureFrame();
      yield return textureFrameRequest;
      var textureFrame = textureFrameRequest.result;
      textureFrame.ReadTextureFromOnCPU(image);
      _graphRunner.AddTextureFrameToInputStream(textureFrame).AssertOk();
      yield return new WaitForEndOfFrame();
    }
  }

  private void OnPoseLandmarksOutput(NormalizedLandmarkList poseLandmarks) {
    if (_annotationController != null && poseLandmarks != null) {
      _annotationController.DrawLater(poseLandmarks);
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
}
