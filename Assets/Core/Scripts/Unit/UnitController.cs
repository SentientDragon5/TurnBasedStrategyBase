using UnityEngine;

/// <summary>
/// Sets the Animation of the character
/// </summary>
public class UnitController : MonoBehaviour
{
    Unit unit;
    Animator anim;
    public int animIndex;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("Index", animIndex);

        unit = GetComponent<Unit>();
        unit.onDie.AddListener(DeathAnim);
    }

    public int dieAnimIndex;
    public void DeathAnim()
    {
        anim.SetFloat("Index", dieAnimIndex);
    }
}
