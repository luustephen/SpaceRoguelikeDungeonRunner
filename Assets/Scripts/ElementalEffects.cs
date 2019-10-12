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
    public const int FIREWATER = 6;
    public const int WATERLIGHTNING = 3;
    public const int FIRELIGHTNING = 5;
    public const int FIREWATERLIGHTNING = 7;

    [Tooltip("ELEMENTLESS = 0 , FIRE = 4 , WATER = 2 , LIGHTNING = 1 , FIREWATER = 6 , WATERLIGHTNING = 3 , FIRELIGHTNING = 5 , FIREWATERLIGHTNING = 7")]
    public int element = 0; //What element does this object start at
    [Tooltip("Does this object change elements when touching another object")]
    public bool changeElementOnTouch = false; //Should this object change/combine elements on touching another object with a different element

    private Material originalMaterial;
    public Material waterMaterial;
    public Material fireMaterial;
    public Material lightningMaterial;
    public Material fireWaterMaterial;
    public Material waterLightningMaterial;
    public Material fireLightningMaterial;
    public Material fireWaterLightningMaterial;
    private SpriteRenderer sprite;
    public GameObject elementChangedBy;

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
        else if (element == FIREWATER)
        {
            sprite.material = fireWaterMaterial;
        }
        else if (element == FIRELIGHTNING)
        {
            sprite.material = fireLightningMaterial;
        }
        else if (element == WATERLIGHTNING)
        {
            sprite.material = waterLightningMaterial;
        }
        else if (element == FIREWATERLIGHTNING)
        {
            sprite.material = fireWaterLightningMaterial;
        }
    }
	
	// Update is called once per frame
	void Update () {
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
        else if (element == FIREWATER)
        {
            sprite.material = fireWaterMaterial;
        }
        else if (element == FIRELIGHTNING)
        {
            sprite.material = fireLightningMaterial;
        }
        else if (element == WATERLIGHTNING)
        {
            sprite.material = waterLightningMaterial;
        }
        else if (element == FIREWATERLIGHTNING)
        {
            sprite.material = fireWaterLightningMaterial;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ElementalEffects otherElementScript;
        if (changeElementOnTouch)
        {
            if (otherElementScript = collision.gameObject.GetComponent<ElementalEffects>())
            {
                AddElement(otherElementScript.element);
                elementChangedBy = collision.gameObject;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ElementalEffects otherElementScript;
        if (changeElementOnTouch)
        {
            if (otherElementScript = collision.gameObject.GetComponent<ElementalEffects>())
            {
                AddElement(otherElementScript.element);
                elementChangedBy = collision.gameObject;
            }
        }
    }

    public void AddElement(int otherElement)
    {
        if (((element & FIRE) == FIRE) && ((otherElement & FIRE) == FIRE)) //If the otherelement is reapplying fire
        {
        }
        if (((element & WATER) == WATER) && ((otherElement & WATER) == WATER)) //If the otherelement is reapplying water
        {
        }
        if (((element & LIGHTNING) == LIGHTNING) && ((otherElement & LIGHTNING) == LIGHTNING)) //If the otherelement is reapplying lightning
        {
        }
        element = element | otherElement;
    }

    public void RemoveElement(int otherElement)
    {
        element = element ^ otherElement; //XOR
    }
}
