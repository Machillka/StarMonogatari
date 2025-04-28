using System.Collections;
using UnityEngine;

public class ItemInteractive : MonoBehaviour
{
    private bool _isAnimating;

    private WaitForSeconds _pause = new WaitForSeconds(0.04f);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isAnimating)
        {
            if (collision.transform.position.x < transform.position.x)
            {
                // 对方在左侧
                StartCoroutine(RotateAnimation(-1));
            }
            else
            {
                StartCoroutine(RotateAnimation(1));
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!_isAnimating)
        {
            if (collision.transform.position.x > transform.position.x)
            {
                // 对方在右侧
                StartCoroutine(RotateAnimation(-1));
            }
            else
            {
                StartCoroutine(RotateAnimation(1));
            }
        }
    }

    /// <summary>
    /// 交互晃动动画实现
    /// </summary>
    /// <param name="direction">1 表示向左移动, -1 表示向右移动</param>
    /// <returns></returns>
    private IEnumerator RotateAnimation(int direction)
    {
        _isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(0).Rotate(0, 0, 2 * direction);
            yield return _pause;
        }

        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(0).Rotate(0, 0, -2 * direction);
            yield return _pause;
        }

        _isAnimating = false;
    }
}
