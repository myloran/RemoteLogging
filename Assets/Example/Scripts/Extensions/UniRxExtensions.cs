#pragma warning disable 4014
using System;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//ReactiveProperty HasValue is a serializable property, make it a method maybe?(it serializes on save)
namespace UniRx {
  public static class UniRxExtensions {
    #region True / False
    public static IDisposable True(this ReactiveBool reactiveBool, Action<bool> action) {
      return reactiveBool
        .Where(_ => _)
        .First()
        .Subscribe(_ => action(_));
    }

    public static IDisposable False(this ReactiveBool reactiveBool, Action<bool> action) {
      return reactiveBool
        .Where(_ => !_)
        .First()
        .Subscribe(_ => action(_));
    }

    public static IDisposable OnTrue(this ReactiveBool reactiveBool, Action<bool> action) {
      return reactiveBool
        .Where(_ => _)
        .Subscribe(_ => action(_));
    }

    public static IDisposable OnFalse(this ReactiveBool reactiveBool, Action<bool> action) {
      return reactiveBool
        .Where(_ => !_)
        .Subscribe(_ => action(_));
    }
    #endregion

    public static IObservable<TSource> Throttle<TSource>(
      this IObservable<TSource> source, 
      int ms
  ) {
      return source.Throttle(
        TimeSpan.FromMilliseconds(ms), 
        Scheduler.DefaultSchedulers.TimeBasedOperations);
    }

    #region Sub action
    public static void Sub(this Toggle toggle, Action<bool> action) {
      toggle
        .OnValueChangedAsObservable()
        .Subscribe(_ => action(_))
        .AddTo(toggle);
    }

    public static void Sub(this Button button, Action<Unit> action) {
      button
        .OnClickAsObservable()
        .Subscribe(_ => action(_))
        .AddTo(button);
    }

    public static void Sub(this Dropdown dropdown, Action<int> action) {
      dropdown.onValueChanged
        .AsObservable()
        .Subscribe(_ => action(_))
        .AddTo(dropdown);
    }

    public static void Sub(this Slider slider, Action<float> action) {
      slider.onValueChanged
        .AsObservable()
        .Subscribe(_ => action(_))
        .AddTo(slider);
    }

    public static void Sub(this InputField input, Action<string> action) {
      input.onValueChanged
        .AsObservable()
        .Subscribe(_ => action(_))
        .AddTo(input);
    }

    public static void OnClickSub(this UIBehaviour ui, Action<PointerEventData> action) {
      ui
        .OnPointerClickAsObservable()
        .Subscribe(_ => action(_))
        .AddTo(ui);
    }

    public static void EscapeSub(this Slider slider) {
      float startingValue;
      CompositeDisposable disposable = new CompositeDisposable();
      slider
        .OnBeginDragAsObservable()
        .Subscribe(_ => {
          disposable = new CompositeDisposable();
          startingValue = slider.value;
          SubToEscape(disposable, slider, startingValue);
        }).AddTo(slider);

      slider
        .OnEndDragAsObservable()
        .Subscribe(_ => disposable.Dispose())
        .AddTo(slider);
    }

    static void SubToEscape(CompositeDisposable disposable, Slider slider, float startingValue) {
      Observable
        .EveryUpdate()
        .Subscribe(__ => {
          if (Input.GetKeyDown(KeyCode.Escape)) {
            slider.value = startingValue;

            EventSystem.current
              .GetComponent<StandaloneInputModule>()
              .DeactivateModule();
          }
        }).AddTo(disposable);
    }

    #endregion
  }

  #region Reactive properties
[Serializable]
  public class ReactiveBool : ReactiveProperty<bool> {
    public ReactiveBool()
            : base() {
    }

    public ReactiveBool(bool initialValue)
            : base(initialValue) {
    }

    public static implicit operator bool(ReactiveBool property) {
      return property.Value;
    }

    public static ReactiveBool operator ++(ReactiveBool property) {
      property.Value = true;
      return property;
    }

    public static ReactiveBool operator --(ReactiveBool property) {
      property.Value = false;
      return property;
    }
  }

  [Serializable]
  public class ReactiveInt : ReactiveProperty<int> {
    public ReactiveInt()
            : base() {
    }

    public ReactiveInt(int initialValue)
            : base(initialValue) {
    }

    public static implicit operator int(ReactiveInt property) {
      return property.Value;
    }

    public static ReactiveInt operator ++(ReactiveInt property) {
      property.Value++;
      return property;
    }

    public static ReactiveInt operator --(ReactiveInt property) {
      property.Value--;
      return property;
    }
  }

  [Serializable]
  public class ReactiveFloat : ReactiveProperty<float> {
    public ReactiveFloat()
            : base() {
    }

    public ReactiveFloat(float initialValue)
            : base(initialValue) {
    }

    public static implicit operator float(ReactiveFloat property) {
      return property.Value;
    }

    public static ReactiveFloat operator ++(ReactiveFloat property) {
      property.Value++;
      return property;
    }

    public static ReactiveFloat operator --(ReactiveFloat property) {
      property.Value--;
      return property;
    }
  }

  [Serializable]
  public class ReactiveString : ReactiveProperty<string> {
    public ReactiveString()
            : base() {
    }

    public ReactiveString(string initialValue)
            : base(initialValue) {
    }

    public static implicit operator string(ReactiveString property) {
      return property.Value;
    }

    //public static ReactiveString operator ++(ReactiveString property) {
    //    property.Value++;
    //    return property;
    //}

    //public static ReactiveString operator --(ReactiveString property) {
    //    property.Value--;
    //    return property;
    //}
  } 
  #endregion

  public static class RX {
    public static IObservable<long> Timer(int ms) {
      return Observable.Timer(TimeSpan.FromMilliseconds(ms));
    }

    public static IObservable<long> Timer(int startingDelayMs, int delayBetweenTicksMs) {
      return Observable.Timer(TimeSpan.FromMilliseconds(startingDelayMs), TimeSpan.FromMilliseconds(delayBetweenTicksMs));
    }
  }
}