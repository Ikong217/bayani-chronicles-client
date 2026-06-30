using UnityEngine;
using System.Collections.Generic;

public class _48_Pro_Male_Exclusive : MonoBehaviour
{
    //Body parts
    private GameObject[] bodies = new GameObject[7];

    //Head parts
    //face
    private GameObject[] faces = new GameObject[3];

    //Hair
    private GameObject[] hairs = new GameObject[4];

    //hair - front hair
    private GameObject[] fHairs = new GameObject[4];

    //beard
    private GameObject[] beards = new GameObject[2];

    //hat
    private GameObject[] hats = new GameObject[4];

    //Character Costumization
    public EnumBodies _eBodies = EnumBodies.Simple1;
    public EnumFaces _eFaces = EnumFaces.Face1;
    public EnumBeard _eBeards = EnumBeard.None;
    public EnumHats _eHats = EnumHats.None;
    public EnumHairs _eHairs = EnumHairs.Hair1;
    public EnumFHairs _eFHairs = EnumFHairs.FrontHair1;

    private void Awake()
    {
        Transform body = transform.Find("Body");

        bodies = _48Pro_Characters.GetChildren(body, bodies.Length);

        Transform head = transform.Find("Head");

        if (head != null)
        {
            Transform face = head.Find("Face");
            if (face != null)
                faces = _48Pro_Characters.GetChildren(face, faces.Length);

            Transform hair = head.Find("Hair");
            if (hair != null)
            {
                hairs = _48Pro_Characters.GetChildren(hair, hairs.Length);

                Transform fHair = hair.Find("FrontHair");
                if (fHair != null)
                    fHairs = _48Pro_Characters.GetChildren(fHair, fHairs.Length);
            }

            Transform beard = head.Find("Beared");
            if (beard != null)
                beards = _48Pro_Characters.GetChildren(beard, beards.Length);

            Transform hat = head.Find("Hat");
            if (hat != null)
                hats = _48Pro_Characters.GetChildren(hat, hats.Length);
        }

        _48Pro_Characters.InitializeObject(bodies, (int)_eBodies);
        _48Pro_Characters.InitializeObject(faces, (int)_eFaces);
        _48Pro_Characters.InitializeObject(hairs, (int)_eHairs);
        _48Pro_Characters.InitializeObject(fHairs, (int)_eFHairs);
        _48Pro_Characters.InitializeObject(beards, (int)_eBeards);
        _48Pro_Characters.InitializeObject(hats, (int)_eHats);
    }


    public enum EnumBodies
    {
        Simple1,
        Simple2,
        Suit1,
        Suit2,
        Suit3,
        Priest1,
        Priest2
    }

    public enum EnumFaces
    {
        Face1,
        Face2,
        Face3
    }

    public enum EnumHairs
    {
        Hair1,
        Hair2,
        Hair3,
        Hair4,
        None
    }

    public enum EnumBeard
    {
        Whole,
        Mustache,
        None,
    }

    public enum EnumHats
    {
        Hat1,
        Hat2,
        SuitHat,
        PriestHat,
        None,
    }

    public enum EnumFHairs
    {
        FrontHair1,
        FrontHair2,
        FrontHair3,
        FrontHair4,
        None,
    }
}


public static class _48Pro_Characters
{
    public static void ActivatePart(GameObject part)
    {
        if (part != null)
        {
            part.SetActive(true);
        }
        else
        {
            Debug.Log("Missing component or null" + part);
        }
    }

    public static void InitializeObject(GameObject[] container, int position)
    {
        if (container == null) return;
        
        for(int i = 0; i < container.Length; i++)
        {
            GameObject obj = container[i];
            if (obj != null)
            {
                if (i == position)
                    ActivatePart(obj);
                else
                    DeactivatePart(obj);
            }
        }
    }

    public static GameObject[] GetChildren(Transform source, int count)
    {
        GameObject[] children = new GameObject[count];

        if (source == null)
        {
            Debug.Log("Missing Component: " + source);
            return children;
        }

        int childCount = source.childCount;
        for (int i = 0; i < count && i < childCount; i++)
        {
            Transform childTransform = source.GetChild(i);
            if (childTransform != null)
            {
                children[i] = childTransform.gameObject;
            }
        }

        return children;
    }

    public static void DeactivatePart(GameObject part)
    {
        if (part != null)
        {
            part.SetActive(false);
        }
        else
        {
            Debug.Log("Missing component or null" + part);
        }
    }

    public static void TogglePart(GameObject part)
    {
        if (part != null)
        {
            part.SetActive(!part.activeSelf);
        }
        else
        {
            Debug.Log("Missing component or null" + part);
        }
    }
}