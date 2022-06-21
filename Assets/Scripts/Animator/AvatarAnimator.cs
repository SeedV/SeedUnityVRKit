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
  public class AvatarAnimator : UpperBodyAnimator {
    public GameObject MouthClose;
    public GameObject MouthSmall;
    public GameObject MouthMid;
    public GameObject MouthLarge;
    public GameObject EyesClose;
    public GameObject EyesOpen;
    public GameObject Eyeslids;

    private void SetObjectVisible(GameObject obj, bool flag) {
      obj.SetActive(flag);
    }

    public override void SetMouth(FaceLandmarks faceLandmarks) {
      SetObjectVisible(MouthClose, MouthShape.Close == faceLandmarks.mouthShape);
      SetObjectVisible(MouthSmall, MouthShape.Small == faceLandmarks.mouthShape);
      SetObjectVisible(MouthMid, MouthShape.Mid == faceLandmarks.mouthShape);
      SetObjectVisible(MouthLarge, MouthShape.Large == faceLandmarks.mouthShape);
    }

    public override void SetEye(bool close) {
      SetObjectVisible(EyesClose, close);
      SetObjectVisible(EyesOpen, !close);
      SetObjectVisible(Eyeslids, !close);
    }
  }
}
