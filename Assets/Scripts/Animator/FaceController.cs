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
using UnityEngine;

namespace SeedUnityVRKit {
  public class FaceController : MonoBehaviour {
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

    public void SetMouth(MouthShape mouthShape) {
      SetObjectVisible(MouthClose, MouthShape.Close == mouthShape);
      SetObjectVisible(MouthSmall, MouthShape.Small == mouthShape);
      SetObjectVisible(MouthMid, MouthShape.Mid == mouthShape);
      SetObjectVisible(MouthLarge, MouthShape.Large == mouthShape);
    }

    public void SetEyes(EyeShape eyeShape) {
      SetObjectVisible(EyesClose, EyeShape.Close == eyeShape);
      SetObjectVisible(EyesOpen, EyeShape.Open == eyeShape);
      SetObjectVisible(Eyeslids, EyeShape.Open == eyeShape);
    }
  }
}
