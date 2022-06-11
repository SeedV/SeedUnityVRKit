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
using UnityEngine;

namespace SeedUnityVRKit {
  class StatsData {
    public int Id { get; set; }
    public int FrameProcessed { get; set; }
    public int FrameRendered { get; set; }
    public int FrameReadTexture { get; set; }
    public int FrameAddToInputStream { get; set; }
    public override string ToString() {
      return $"SystemStats: [{Id}] FrameReadTexture: {FrameReadTexture} " +
             $"FrameAddToInputStream: {FrameAddToInputStream} FrameProcessed: {FrameProcessed} " +
             $"FrameRendered: {FrameRendered}";
    }
  }

  public class SystemStats : MonoBehaviour {
    private const int _maxBuffer = 20;
    private const int _delaySeconds = 3;
    private const int _sampleInterval = 1;
    private int _frameIdCounter = 0;
    private int _frameProcessedCounter = 0;
    private int _frameRenderedCounter = 0;
    private int _frameReadTextureCounter = 0;
    private int _frameAddToInputStreamCounter = 0;
    private Queue<StatsData> _statsQueue = new Queue<StatsData>();

    public IEnumerator Start() {
      Debug.Log($"SystemStats: Waiting for {_delaySeconds} seconds to stablize.");
      yield return new WaitForSeconds(_delaySeconds);
      Debug.Log("SystemStats: Counters started");
      while (true) {
        _frameProcessedCounter = 0;
        _frameRenderedCounter = 0;
        _frameReadTextureCounter = 0;
        _frameAddToInputStreamCounter = 0;
        yield return new WaitForSeconds(_sampleInterval);
        StatsData data = new StatsData {
          Id = _frameIdCounter,
          FrameProcessed = _frameProcessedCounter,
          FrameRendered = _frameRenderedCounter,
          FrameAddToInputStream = _frameAddToInputStreamCounter,
          FrameReadTexture = _frameReadTextureCounter,
        };
        _statsQueue.Enqueue(data);
        if (_statsQueue.Count > _maxBuffer) {
          _statsQueue.Dequeue();
        }
        _frameIdCounter++;
      }
    }

    public void OnApplicationQuit() {
      foreach (var stats in _statsQueue) {
        Debug.Log(stats.ToString());
      }
    }

    public void IncrementFrameProcessed() {
      _frameProcessedCounter++;
    }

    public void IncrementFrameRendered() {
      _frameRenderedCounter++;
    }

    public void IncrementFrameReadTexture() {
      _frameReadTextureCounter++;
    }

    public void IncrementFrameAddToInputStream() {
      _frameAddToInputStreamCounter++;
    }
  }
}
