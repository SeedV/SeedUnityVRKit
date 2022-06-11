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

namespace SeedUnityVRKit {

  public abstract class BaseTracker : MonoBehaviour {
    public SystemStats systemStats;
    private const string _tag = nameof(BaseTracker);
    public enum InferenceMode {
      CPU,
      GPU,
    }
    [Tooltip("Inference mode.")]
    [SerializeField]
    private InferenceMode _preferableInferenceMode;
    [Tooltip("Reference to the internal texture frame pool.")]
    [SerializeField]
    private TextureFramePool _textureFramePool;
    [SerializeField]
    private Texture _sourceTexture;
    [Tooltip("Whether to flip the source image horizontally before processing.")]
    [SerializeField]
    private bool _hFlip;
    [Tooltip("Whether to flip the source image vertically before processing.")]
    [SerializeField]
    private bool _vFlip;
    [Tooltip("Angles to rotate the source image.")]
    [SerializeField]
    private int _rotation;
    protected HolisticTrackingGraph _graphRunner;
    private Coroutine _coroutine;
    private InferenceMode _inferenceMode;

    // <summary>Attaches any event handler for callbacks. </summary>
    public abstract void AddEventHandler();

    public IEnumerator Start() {
      _graphRunner = GetComponent<HolisticTrackingGraph>();
      AssetLoader.Provide(new StreamingAssetsResourceManager());

      Logger.LogInfo(_tag, "Starting mediapipe");
      DecideInferenceMode();
      if (_inferenceMode == InferenceMode.GPU) {
        Logger.LogInfo(_tag, "Initializing GPU resources...");
        yield return GpuManager.Initialize();
      }
      _textureFramePool.ResizeTexture(_sourceTexture.width, _sourceTexture.height,
                                      TextureFormat.RGBA32);
      var graphInitRequest = _graphRunner.WaitForInit(RunningMode.Async);
      yield return graphInitRequest;
      if (graphInitRequest.isError) {
        Debug.Log(graphInitRequest.error);
        yield break;
      }
      AddEventHandler();
      Logger.LogInfo(_tag, "Graph Runner Init!");
      SidePacket sidePacket = _graphRunner.BuildSidePacket(_rotation, _hFlip, _vFlip);
      _graphRunner.StartRun(sidePacket);
      Logger.LogInfo(_tag, "Graph Runner started in async mode!");

      _coroutine = StartCoroutine(ProcessImage(_sourceTexture));
    }

    public void OnDestroy() {
      if (_coroutine != null)
        StopCoroutine(_coroutine);
    }

    private IEnumerator ProcessImage(Texture image) {
      Logger.LogVerbose(_tag, "Process image");
      while (true) {
        if (!_textureFramePool.TryGetTextureFrame(out var textureFrame)) {
          yield return new WaitForEndOfFrame();
          continue;
        }
        textureFrame.ReadTextureFromOnCPU(image);
        systemStats?.IncrementFrameReadTexture();
        _graphRunner.AddTextureFrameToInputStream(textureFrame);
        systemStats?.IncrementFrameAddToInputStream();
        yield return null;
      }
    }

    private void DecideInferenceMode() {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
      if (_preferableInferenceMode == InferenceMode.GPU) {
        Debug.Log(
            "Current platform does not support GPU inference mode, so falling back to CPU mode");
      }
      _inferenceMode = InferenceMode.CPU;
#else
      _inferenceMode = _preferableInferenceMode;
#endif
    }
  }
}
