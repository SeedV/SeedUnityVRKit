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
  // The names of each BlazePose joints in the same order.
  // https://google.github.io/mediapipe/images/mobile/pose_tracking_full_body_landmarks.png
  public class Landmarks {
    public const int Nose = 0;
    public const int LeftEyeInner = 1;
    public const int LeftEye = 2;
    public const int LeftEyeOuter = 3;
    public const int RightEyeInner = 4;
    public const int RightEye = 5;
    public const int RightEyeOuter = 6;
    public const int LeftEar = 7;
    public const int RightEar = 8;
    public const int MouthLeft = 9;
    public const int MouthRight = 10;
    public const int LeftShoulder = 11;
    public const int RightShoulder = 12;
    public const int LeftElbow = 13;
    public const int RightElbow = 14;
    public const int LeftWrist = 15;
    public const int RightWrist = 16;
    public const int LeftPinky = 17;
    public const int RightPinky = 18;
    public const int LeftIndex = 19;
    public const int RightIndex = 20;
    public const int LeftThumb = 21;
    public const int RightThumb = 22;
    public const int LeftHip = 23;
    public const int RightHip = 24;
    public const int LeftKnee = 25;
    public const int RightKnee = 26;
    public const int LeftAnkle = 27;
    public const int RightAnkle = 28;
    public const int LeftHeel = 29;
    public const int RightHeel = 30;
    public const int LeftFootIndex = 31;
    public const int RightFootIndex = 32;

    public const int PoseCount = 33;

    // Estimated landmarks
    public const int Hip = 33;
    public const int Neck = 34;

    public const int Total = 35;

    public int Id { get; set; }
    public Quaternion Rotation { get; set; }
  };
}
