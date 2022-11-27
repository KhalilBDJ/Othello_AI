using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disc : MonoBehaviour
{

    [SerializeField] 
    private PlayerEnum up;

    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Flip()
    {
        if (up == PlayerEnum.Black)
        {
            _animator.Play("FlipDisc");
            up = PlayerEnum.White;
        }
        else
        {
            _animator.Play("WhiteToBlack");
            up = PlayerEnum.Black;
        }
    }

    public void Twitch()
    {
        _animator.Play("TwitchDisc");
    }
}
