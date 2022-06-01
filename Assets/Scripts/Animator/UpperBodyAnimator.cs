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

using Mediapipe;
using Mediapipe.Unity;
using UnityEngine;

namespace SeedUnityVRKit {
  // <summary>An animator to visualize upper body and face.</summary>
  public class UpperBodyAnimator : MonoBehaviour {
    [Tooltip("Reference to MTH_DEF game object in UnityChan model.")]
    public SkinnedMeshRenderer MthDefRef;
    [Tooltip("Max rotation angle in degree.")]
    [Range(0, 45f)]
    public float MaxRotationThreshold = 40f;
    [Tooltip("Screen width used as to scale the recognized normalized landmarks.")]
    public float ScreenWidth = 1920;
    [Tooltip("Screen height used as to scale the recognized normalized landmarks.")]
    public float ScreenHeight = 1080;
    /// <summary>The neck joint to control head rotation.</summary>
    private Transform _neck;
    /// <summary>Face landmark recognizer.</summary>
    private FaceLandmarksRecognizer _faceLandmarksRecognizer;
    private NormalizedLandmarkList _normalizedLandmarkList;

    void Start() {
      var anim = GetComponent<Animator>();

      _neck = anim.GetBoneTransform(HumanBodyBones.Neck);
      _faceLandmarksRecognizer = new FaceLandmarksRecognizer(ScreenWidth, ScreenHeight);
    }

    void LateUpdate() {
      if (_normalizedLandmarkList != null) {
        FaceLandmarks faceLandmarks = _faceLandmarksRecognizer.recognize(_normalizedLandmarkList);
        _neck.localEulerAngles = ClampFaceRotation(faceLandmarks.FaceRotation);
        SetMouth(faceLandmarks.MouthAspectRatio);
      }
    }

    private Vector3 ClampFaceRotation(Vector3 rotation) {
      return new Vector3(rotation.x,  // Do not clamp x
                         Mathf.Clamp(rotation.y, -MaxRotationThreshold, MaxRotationThreshold),
                         Mathf.Clamp(rotation.z, -MaxRotationThreshold, MaxRotationThreshold));
    }

    private void SetMouth(float ratio) {
      MthDefRef.SetBlendShapeWeight(2, ratio * 100);
    }

    public void OnFaceLandmarksOutput(object stream,
                                      OutputEventArgs<NormalizedLandmarkList> eventArgs) {
      _normalizedLandmarkList = eventArgs.value;
    }
  }
}
