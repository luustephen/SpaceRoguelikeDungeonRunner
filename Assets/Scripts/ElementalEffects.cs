using UnityEngine;

public class ElementalEffects : MonoBehaviour {

    /*ELEMENTAL TLDR
     * Fire Water Lightning
     * FWL
     * 000 = 0 = None
     * 100 = 4 = Fire
     * 010 = 2 = Water
     * 001 = 1 = Lightning
     * 110 = 6 = Steam
     * 101 = 5 = Fiery Lightning
     * 011 = 3 = Water Lightning
     * 111 = 7 = Storm?
     * 
     * 
     * */

    public const int ELEMENTLESS = 0;
    public const int FIRE = 4;
    public const int WATER = 2;
    public const int LIGHTNING = 1;
    public int element = 0; //What element does this object start at
    private Material originalMaterial;
    public Material waterMaterial;
    public Material fireMaterial;
    public Material lightningMaterial;
    private SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        originalMaterial = sprite.material;
        if (element == ELEMENTLESS)
        {
            sprite.material = originalMaterial;
        }
        else if (element == WATER)
        {
            sprite.material = waterMaterial;
        }
        else if (element == FIRE)
        {
            sprite.material = fireMaterial;
        }
        else if (element == LIGHTNING)
        {
            sprite.material = lightningMaterial;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if(element == ELEMENTLESS)
        {
            sprite.material = originalMaterial;
        }
        else if(element == WATER)
        {
            sprite.material = waterMaterial;
        }
        else if (element == FIRE)
        {
            sprite.material = fireMaterial;
        }
        else if (element == LIGHTNING)
        {
            sprite.material = lightningMaterial;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ElementalEffects otherElementScript;
        if (otherElementScript = collision.gameObject.GetComponent<ElementalEffects>())
        {
            AddElement(otherElementScript.element);
        }
    }

    public void AddElement(int otherElement)
    {
        print(gameObject.tag);
        if (((element & FIRE) == FIRE) && ((otherElement & FIRE) == FIRE)) //If the otherelement is reapplying fire
        {
            print("reapply fire");
        }
        if (((element & WATER) == WATER) && ((otherElement & WATER) == WATER)) //If the otherelement is reapplying water
        {
            print("reapply water");
        }
        if (((element & LIGHTNING) == LIGHTNING) && ((otherElement & LIGHTNING) == LIGHTNING)) //If the otherelement is reapplying lightning
        {
            print("reapply light");
        }
        element = element | otherElement;
        print("add element:" + element);
    }

    public void RemoveElement(int otherElement)
    {
        element = element ^ otherElement; //XOR
        print("remove element:" + element);
    }
}
