using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPresenter : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private SpriteRenderer sr;

    [SerializeField]
    private Image img;

    private Animator animator;
    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        animator.SetBool(PlayerPrefs.GetString("Character","isMage"), true);
    }


    private void FixedUpdate()
    {
        img.sprite = sr.sprite;
    }
}
