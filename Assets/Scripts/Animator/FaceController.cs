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
    private const string MthDefConst = "MTH_DEF";
    private const string EyeDefConst = "EYE_DEF";
    private const string ElDefConst = "EL_DEF";
    private SkinnedMeshRenderer _mouthMeshRenderer;
    private SkinnedMeshRenderer _eyeMeshRenderer;
    private SkinnedMeshRenderer _elMeshRenderer;

    private void SetObjectVisible(GameObject obj, bool flag) {
      obj.SetActive(flag);
    }

    public void SetMouth(MouthShape mouthShape, float ratio) {
      if (_mouthMeshRenderer != null) {
        _mouthMeshRenderer.SetBlendShapeWeight(2, ratio * 100);
      } else {
        SetObjectVisible(MouthClose, MouthShape.Close == mouthShape);
        SetObjectVisible(MouthSmall, MouthShape.Small == mouthShape);
        SetObjectVisible(MouthMid, MouthShape.Mid == mouthShape);
        SetObjectVisible(MouthLarge, MouthShape.Large == mouthShape);
      }

    }

    public void SetEyes(EyeShape eyeShape) {
      if (_eyeMeshRenderer != null && _elMeshRenderer != null) {
        if (eyeShape == EyeShape.Close) {
          _eyeMeshRenderer.SetBlendShapeWeight(6, 100);
          _elMeshRenderer.SetBlendShapeWeight(6, 100);
        } else {
          _eyeMeshRenderer.SetBlendShapeWeight(6, 0);
          _elMeshRenderer.SetBlendShapeWeight(6, 0);
        }
      } else {
        SetObjectVisible(EyesClose, EyeShape.Close == eyeShape);
        SetObjectVisible(EyesOpen, EyeShape.Open == eyeShape);
        SetObjectVisible(Eyeslids, EyeShape.Open == eyeShape);
      }

    }

    void Start() {
      _mouthMeshRenderer = GameObject.Find(MthDefConst).GetComponent<SkinnedMeshRenderer>();
      _eyeMeshRenderer = GameObject.Find(EyeDefConst).GetComponent<SkinnedMeshRenderer>();
      _elMeshRenderer = GameObject.Find(ElDefConst).GetComponent<SkinnedMeshRenderer>();
    }
  }
}
