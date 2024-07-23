using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStageRun {
    bool IsRunning { get; set; }
    void Run();
}
