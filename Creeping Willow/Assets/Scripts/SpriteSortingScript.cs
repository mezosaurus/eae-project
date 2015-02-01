using UnityEngine;
using System.Linq;

public class SpriteSortingScript : MonoBehaviour
{
    private void SortBackgroundSprites()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();

        // Sort background layer first
        sprites = sprites.Where(item => item.sortingLayerName == "Background").OrderByDescending(x => x.gameObject.transform.position.y).ThenBy(x => x.gameObject.transform.position.x).ToArray();

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sortingOrder = i;
        }

        // Sort upper background layer next
        sprites = sprites.Where(item => item.sortingLayerName == "UpperBackground").OrderByDescending(x => x.gameObject.transform.position.y).ThenBy(x => x.gameObject.transform.position.x).ToArray();

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sortingOrder = i;
        }
    }

    private void SortForegroundSprites()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();

        sprites = sprites.Where(item => item.sortingLayerName == "").OrderByDescending(x => x.gameObject.transform.position.y).ToArray();

        int player = -1;
        int j = 0;

        for (int i = 0; i < sprites.Length; i++)
        {            
            if (sprites[i].gameObject.tag == "PlayerBody") { j += 8; player = i; }

            sprites[i].sortingOrder = j++;

            if (sprites[i].gameObject.tag == "PlayerBody") j += 8;
        }

        if (Application.isPlaying)
        {
        	if (player >= 0) sprites[player].transform.parent.GetComponent<PossessableTree>().UpdateSorting();
        }
    }
#if UNITY_EDITOR
    // Use this for initialization
    void Start ()
    {
        if(Application.isPlaying) SortBackgroundSprites();
    }
	
    // Update is called once per frame
    void Update ()
    {
        if (!Application.isPlaying) SortBackgroundSprites();
    }

    void LateUpdate()
    {
        SortForegroundSprites();
    }
#else
    // Use this for initialization
    void Start ()
    {
        SortBackgroundSprites();
    }

    void LateUpdate()
    {
        SortForegroundSprites();
    }
#endif
}