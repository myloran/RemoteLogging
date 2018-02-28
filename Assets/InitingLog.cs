using UnityEngine;

public class InitingLog : MonoBehaviour {
  void Start() {
    if (!App.IsInit) App.Init();
  }
}
