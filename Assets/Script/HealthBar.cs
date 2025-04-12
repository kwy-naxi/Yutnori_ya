using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Reflection;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarFillImage;
    [SerializeField] private Image _healthBarTrailingRillImage;
    [SerializeField] private float _trailDely = 0.4f;

    [SerializeField] private float _maxHealth = 100f;

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;

        _healthBarFillImage.fillAmount = 1f;
        _healthBarTrailingRillImage.fillAmount = 1f;
    }


    private void Update()
    {
        if (keyboard.current.spaceKey.wasPressedThisFrame)
        {
            DrainHealthBar();
        }
    }

    private void DrainHealthBar()
    {
        _currentHealth -= 10f;
        float ratio = _currentHealth / _maxHealth;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_healthBarFillImage.DOFillAmount(ratio, 0.25f))
            .SetEase(Ease.InOutSine);
        sequence.AppendInterval(_trailDely);
        sequence.Append(_healthBarTrailingRillImage.DOFillAmount(ratio, 0.3f))
            .SetEase(Ease.InOutSine);

        sequence.Play();
    }
}
