using Lumina.Essentials.Attributes;
using UnityEngine;

public class HealthbarManager : SingletonPersistent<HealthbarManager>
{
    [Header("Serialized References")]
    [SerializeField, ReadOnly] Healthbar playerOne;
    [SerializeField, ReadOnly] Healthbar playerTwo;
    
    [Header("Optional Parameters")]
    [SerializeField] float placeholder;

    // Properties
    
    /// <summary>
    /// The healthbar of Player ONE.
    /// <seealso cref="PlayerTwo"/>
    /// <seealso cref="Left"/>
    /// </summary>
    public Healthbar PlayerOne
    {
        get => playerOne;
        set => playerOne = value;
    }
    
    /// <summary>
    /// The healthbar of Player TWO.
    /// <seealso cref="PlayerOne"/>
    /// /// <seealso cref="Right"/>
    /// </summary>
    public Healthbar PlayerTwo
    {
        get => playerTwo;
        set => playerTwo = value;
    }
    
    /// <summary> An alternate name for <see cref="PlayerOne"/>. </summary>
    public Healthbar Left => PlayerOne;
    /// <summary> An alternate name for <see cref="PlayerTwo"/>. </summary>
    public Healthbar Right => PlayerTwo;

    /// <summary>
    /// Adjusts the value of the healthbars.
    /// <remarks> !! This method is not intended to be the one that damages players in the final product, this is simply for debugging purposes. </remarks>
    /// </summary>
    /// <param name="isPlayerOne"> Player ONE is on the LEFT, while Player TWO is on RIGHT. </param>
    /// <param name="newHealth"> The new health to adjust the healthbar to. </param>
    /// <param name="deductionRate"> The amount to reduce the health by each frame. </param>
    public void AdjustHealthbar(bool isPlayerOne, int newHealth, float deductionRate)
    {
        switch (isPlayerOne)
        {
            case true:
                PlayerOne.Slider.value = Mathf.MoveTowards(PlayerOne.Slider.value, newHealth, deductionRate * Time.deltaTime);
                break;
    
            default:
                PlayerTwo.Slider.value = Mathf.MoveTowards(PlayerTwo.Slider.value, newHealth, deductionRate * Time.deltaTime);
                break;
        }
    }
}