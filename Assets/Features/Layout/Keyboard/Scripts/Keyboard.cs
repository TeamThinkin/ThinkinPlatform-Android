using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    private class KeyInfo
    {
        public float KeyDownTime;
        public bool IsHandled;

        public KeyInfo(float KeyDownTime)
        {
            this.KeyDownTime = KeyDownTime;
            this.IsHandled = false;
        }
    }

    [SerializeField] private KeyboardLayout _layout;
    public KeyboardLayout Layout => _layout;

    [SerializeField] private Transform _sizeReference;
    public Transform SizeReference => _sizeReference;

    [SerializeField] private float LongPressDuration = 0.25f;
    [SerializeField] private AnimationCurve ButtonAnimationCurve;
    [SerializeField] private float ButtonAnimationDuration = 0.5f;
    [SerializeField] private AudioClip ButtonDownAudio;
    [SerializeField] private AudioClip ButtonUpAudio;
    [SerializeField] private AudioClip ButtonLongPressAudio;

    public Transform ButtonContainer;
    public KeyboardButton[] Buttons;
    public bool IsCapitals { get; private set; }

    public EditableText Text = new EditableText();

    private Dictionary<KeyboardButton, KeyInfo> keyDownTime = new Dictionary<KeyboardButton, KeyInfo>();
    private bool isOpen;

    public static Keyboard Instance { get; private set; }

    public IFocusItem CurrentFocusItem => FocusManager.CurrentFocusItem;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        isOpen = false;
    }

    public void OnKeyDown(KeyboardButton Button)
    {
        keyDownTime.Add(Button, new KeyInfo(Time.time));
        Button.AudioPlayer.PlayOneShot(ButtonDownAudio);
    }

    private void Update()
    {
        foreach(var entry in keyDownTime)
        {
            if(!entry.Value.IsHandled && entry.Key.KeyInfo.Special == SpecialKeyboardKey.None && Time.time - entry.Value.KeyDownTime >= LongPressDuration) //Check for long press
            {
                entry.Value.IsHandled = true;
                Text.AddText(entry.Key.KeyInfo.SecondaryKey);
                entry.Key.AudioPlayer.PlayOneShot(ButtonLongPressAudio);
            }
        }
    }

    public void OnKeyUp(KeyboardButton Button)
    {
        if (!keyDownTime.ContainsKey(Button)) return;

        var keyInfo = keyDownTime[Button];
        keyDownTime.Remove(Button);

        Button.AudioPlayer.PlayOneShot(ButtonUpAudio);

        if (keyInfo.IsHandled) return;

        switch(Button.KeyInfo.Special)
        {
            case SpecialKeyboardKey.Close:
                Close();
                break;
            case SpecialKeyboardKey.Backspace:
                Text.Backspace();
                break;
            case SpecialKeyboardKey.Shift:
                IsCapitals = !IsCapitals;
                updateButtonText();
                break;
            case SpecialKeyboardKey.None:
                Text.AddText(IsCapitals ? Button.KeyInfo.MainKey.ToUpper() : Button.KeyInfo.MainKey);
                break;
        }
    }

    private void updateButtonText()
    {
        foreach(var button in Buttons)
        {
            button.UpdateText();
        }
    }

    private Coroutine animateCoroutine;
    private Vector3 sourcePosition;
    public void ShowForInput(IFocusItem item)
    {
        if (FocusManager.CurrentFocusItem == item && isOpen)
        {
            FocusManager.ClearFocus();
            Close();
            return;
        }
        
        FocusManager.SetFocus(item);

        var head = Camera.main.transform;
        var targetPosition = head.position + head.forward * 0.3f + head.up * -0.2f;
        var targetRot = Quaternion.LookRotation(head.position - targetPosition);
        
        if (isOpen)
        {
            var startPosition = transform.position;
            var startRotation = transform.rotation;

            //The keyboard is already open, just move to new location
            AnimationHelper.StartAnimation(this, ref animateCoroutine, ButtonAnimationDuration * 0.5f, 0, 1, t =>
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                transform.rotation = Quaternion.Slerp(startRotation, targetRot, t);
            }, () =>
            {
                sourcePosition = ButtonContainer.InverseTransformPoint(item.transform.position);
            }, ButtonAnimationCurve);
        }
        else
        {
            //They keyboard was closed, animate open from source
            transform.position = targetPosition;
            transform.rotation = targetRot;
            gameObject.SetActive(true);
            sourcePosition = ButtonContainer.InverseTransformPoint(item.transform.position);

            AnimationHelper.StartAnimation(this, ref animateCoroutine, ButtonAnimationDuration, 0, 1, t =>
            {
                
                var preservedState = Random.state;
                Random.InitState(123);
                foreach (var button in Buttons)
                {
                    var offset = Random.Range(0, 0.5f);
                    var localT = t.Remap(0, offset, 0, 1, true);
                    button.transform.localPosition = Vector3.Lerp(sourcePosition, button.LayoutLocalPosition, localT);
                    button.transform.localScale = Vector3.Lerp(Vector3.zero, button.LayoutLocalScale, localT);
                }
                Random.state = preservedState;
            }, null, ButtonAnimationCurve);
        }
        isOpen = true;
    }

    public void Close()
    {
        isOpen = false;
        AnimationHelper.StartAnimation(this, ref animateCoroutine, ButtonAnimationDuration * 0.7f, 0, 1, t =>
        {
            var preservedState = Random.state;
            Random.InitState(124);
            foreach (var button in Buttons)
            {
                var offset = Random.Range(0, 0.5f);
                var localT = t.Remap(0, offset, 0, 1, true);
                button.transform.localPosition = Vector3.Lerp(button.LayoutLocalPosition, sourcePosition, localT);
                button.transform.localScale = Vector3.Lerp(button.LayoutLocalScale, Vector3.zero, localT);
            }
            Random.state = preservedState;
        }, () => gameObject.SetActive(false), ButtonAnimationCurve);
    }
}
