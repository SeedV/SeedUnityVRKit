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

using UnityEngine;

namespace SeedUnityVRKit {
  public class WebcamController : MonoBehaviour {
    [SerializeField]
    private string _cameraSource;
    [SerializeField]
    private Vector2Int _resolution = new Vector2Int(1080, 1080);
    [SerializeField]
    private bool _hFlip = false;
    [SerializeField]
    private RenderTexture _buffer;
    private WebCamTexture _webcam;

    public void OnEnable() {
      _webcam = new WebCamTexture(_cameraSource, _resolution.x, _resolution.y, 30);
      _webcam.Play();
    }

    public void OnDisable() {
      if (_webcam)
        Destroy(_webcam);
    }

    public void Update() {
      if (!_webcam.didUpdateThisFrame)
        return;
      bool vFlip = _webcam.videoVerticallyMirrored;
      Vector2 scale = new Vector2(_hFlip ? -1 : 1, vFlip ? -1 : 1);
      Vector2 offset = new Vector2(_hFlip ? 1 : 0 / 2, vFlip ? 1 : 0);
      Graphics.Blit(_webcam, _buffer, scale, offset);
    }
  }
}
