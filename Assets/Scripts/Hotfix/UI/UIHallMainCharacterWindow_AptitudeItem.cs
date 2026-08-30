using UnityEngine;
using UnityEngine.UI;

public class UIHallMainCharacterWindow_AptitudeItem : UIWindowItem
{
    [SerializeField, AutoReference]
    private Image _IconImg;
    [SerializeField, AutoReference]
    private Slider _AptitudeSlider;

    public override void Unload()
    {
        _IconImg.UnloadSprite();
        base.Unload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
    }
    public void Setting(EnEntityAttributeId attId, int sliderValue)
    {
        var attCfg = GameSchedule.Instance.GetAttributeCfg0((int)attId);
        _IconImg.LoadSprirte(attCfg.nIconAssetId);
        _AptitudeSlider.value = Mathf.Clamp01(sliderValue);
        SetSliderValue(sliderValue);
    }
    public void SetSliderValue(int value)
    {
        _AptitudeSlider.value = Mathf.Clamp01(value / 100f);
    }
}
