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
using UnityEngine;

public abstract class BaseTracker : MonoBehaviour {
  private const string _TAG = nameof(BaseTracker);
  public enum InferenceMode {
    CPU,
    GPU,
  }
  [SerializeField] private InferenceMode _preferableInferenceMode;
  [SerializeField] private TextureFramePool _textureFramePool;
  [SerializeField] private Texture _sourceTexture;
  [SerializeField] private bool _hFlip;
  [SerializeField] private bool _vFlip;
  [SerializeField] private int _rotation;
  private Coroutine _coroutine;
  private InferenceMode _inferenceMode;

  public abstract WaitForResult CreateGraphInitRequest();

  public abstract void StartGraph(int rotation, bool hFlip, bool vFlip);

  public abstract void AddEventHandler();

  public abstract void ProcessTextureFrame(TextureFrame textureFrame);

  public virtual IEnumerator Start() {
    AssetLoader.Provide(new StreamingAssetsResourceManager());

    Logger.LogInfo(_TAG, "Starting mediapipe");
    DecideInferenceMode();
    if (_inferenceMode == InferenceMode.GPU) {
      Logger.LogInfo(_TAG, "Initializing GPU resources...");
      yield return GpuManager.Initialize();
    }
    var graphInitRequest = CreateGraphInitRequest();
    _textureFramePool.ResizeTexture(_sourceTexture.width, _sourceTexture.height, TextureFormat.RGBA32);
    yield return graphInitRequest;
    if (graphInitRequest.isError) {
      Debug.Log(graphInitRequest.error);
      yield break;
    }
    AddEventHandler();
    Logger.LogInfo(_TAG, "Graph Runner Init!");
    StartGraph(_rotation, _hFlip, _vFlip);
    Logger.LogInfo(_TAG, "Graph Runner started in async mode!");

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
      ProcessTextureFrame(textureFrame);
      yield return new WaitForEndOfFrame();
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
