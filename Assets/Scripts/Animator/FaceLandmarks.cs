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

namespace SeedUnityVRKit {
  public enum MouthShape {
    Close = 0,
    Small = 1,
    Mid = 2,
    Large = 3,
  }

  public enum EyeShape {
    Close = 0,
    Open = 1,
  }

  public class FaceLandmarks {
    public MouthShape MouthShape { get; set; }
    public EyeShape LeftEyeShape { get; set; }
    public EyeShape RightEyeShape { get; set; }
  }
}