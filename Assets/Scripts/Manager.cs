using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Manager : MonoBehaviour
{
    #region +
    public GameObject numberPrefab;//数字的预设
    public GameObject welcomePage;//欢迎界面
    public GameObject gameWinPage;//游戏胜利界面
    public GameObject gameLosePage;//游戏失败界面
    public GameObject exitMessagePrefab;//退出游戏的提示
    public Texture musicOn;//播放音乐时显示的图片
    public Texture musicOff;//不播放音乐时显示的图片
    public UITexture music;//显示播放音乐的UITexture

    public Number[,] Numbers = new Number[4, 4];//保存方格中数字的数组
    [HideInInspector]
    public List<Number> IsMovingNumbers = new List<Number>();//正在移动中的数字集合
    [HideInInspector]
    public bool hasMove = false;//判断时候有数字发生了移动
    [HideInInspector]
    public bool canControl = false;//游戏是否可以控制
    #endregion

    #region -
    private AudioSource audioSource;
    private bool playerMusic;//是否播放背景音乐
    private GameObject exitMessage;//退出游戏的实例化对象
    #endregion

    #region 单例
    public static Manager _i;

    void Awake()
    {
        _i = this;
    }
    #endregion

    #region Unity内置函数
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();

        Instantiate(numberPrefab);
        Instantiate(numberPrefab);

        playerMusic = PlayerPrefs.GetInt("playMusic", 1) == 1 ? true : false;

        if (playerMusic)
        {
            audioSource.Play();
            music.mainTexture = musicOn;
        }
        else
        {
            audioSource.Stop();
            music.mainTexture = musicOff;
        }
    }
    void Update()
    {
        //手机上的返回键对应的是PC上的ESC键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitMessage == null)
            {
                exitMessage = Instantiate(exitMessagePrefab) as GameObject;
                StartCoroutine("reaetQuitMessage");
            }
            else
            {
                Application.Quit();
            }
        }

        if (canControl && IsMovingNumbers.Count == 0)//没有数字正在移动的时候才能执行这里的操作
        {
            int dirX = 0;
            int dirY = 0;

            if (Input.GetKeyDown("up"))
            {
                dirY = 1;
            }
            else if (Input.GetKeyDown("down"))
            {
                dirY = -1;
            }
            else if (Input.GetKeyDown("left"))
            {
                dirX = -1;
            }
            else if (Input.GetKeyDown("right"))
            {
                dirX = 1;
            }

            //移动端的触屏控制游戏
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

                if (Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y))
                {
                    if (touchDeltaPosition.x > 10)
                    {
                        dirX = 1;
                    }
                    else if (touchDeltaPosition.x < -10)
                    {
                        dirX = -1;
                    }
                }
                else
                {
                    if (touchDeltaPosition.y > 10)
                    {
                        dirY = 1;
                    }
                    else if (touchDeltaPosition.y < -10)
                    {
                        dirY = -1;
                    }
                }
            }

            MoveNumbers(dirX, dirY);
        }

        //每次移动完后实例化一个新的数字
        if (hasMove && IsMovingNumbers.Count == 0)
        {
            Instantiate(numberPrefab);
            hasMove = false;

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (Numbers[x, y] != null)
                    {
                        Numbers[x, y].hasMixed = false;
                    }
                }
            }
        }
    }
    #endregion

    #region +CloseWelcomeToPlayGameScene关闭欢迎界面跳转到游戏场景
    public void CloseWelcomeToPlayGameScene()
    {
        canControl = true;
        welcomePage.SetActive(false);
    }
    #endregion

    #region +MoveNumbers控制数字的移动,不实现数字的合并
    public void MoveNumbers(int dirX, int dirY)
    {
        if (dirX == 1)//右
        {
            for (int y = 0; y < 4; y++)//到4表示4行都能向右移动
            {
                for (int x = 2; x >= 0; x--)//从2开始，表示除了最右边的一列其余3列都能向右移动
                {
                    if (Numbers[x, y] != null)
                    {
                        Numbers[x, y].Move(dirX, dirY);
                    }
                }
            }
        }
        else if (dirX == -1)//左
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 1; x < 4; x++)
                {
                    if (Numbers[x, y] != null)
                    {
                        Numbers[x, y].Move(dirX, dirY);
                    }
                }
            }
        }
        else if (dirY == 1)//上
        {
            for (int x = 0; x < 4; x++)//向上有4列都可以移动
            {
                for (int y = 2; y >= 0; y--)//但只有除最上面的那一行的其余3行才能移动
                {
                    if (Numbers[x, y] != null)
                    {
                        Numbers[x, y].Move(dirX, dirY);
                    }
                }
            }
        }
        else if (dirY == -1)//下
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 1; y < 4; y++)//除了最下面一行其余3行都能移动
                {
                    if (Numbers[x, y] != null)
                    {
                        Numbers[x, y].Move(dirX, dirY);
                    }
                }
            }
        }
    }
    #endregion

    #region +isEmpty用来判断当前传进来的这个位置上有没有数字
    public bool isEmpty(int x, int y)
    {
        if (x < 0 || x >= 4 || y < 0 || y >= 4)//超出了4X4格子边界
        {
            return false;//返回false表示不为空
        }
        else if (Numbers[x, y] != null)//当前这个位置上有数字
        {
            return false;//也是返回不为空
        }
        else//除此之外全部都是空的
        {
            return true;//表示当前传进来的这个位置是可以用的
        }
    }
    #endregion

    #region +ToggleMusc用来控制音乐的播放
    public void ToggleMusc()
    {
        if (playerMusic)
        {
            playerMusic = false;
            audioSource.Stop();
            music.mainTexture = musicOff;
            PlayerPrefs.SetInt("playMusic", 0);
        }
        else
        {
            playerMusic = true;
            audioSource.Play();
            music.mainTexture = musicOn;
            PlayerPrefs.SetInt("playMusic", 1);
        }
    }
    #endregion

    #region +Restart重新开始游戏
    public void Restart()
    {
        canControl = true;

        gameLosePage.SetActive(false);
        gameWinPage.SetActive(false);

        //清空界面上的所有数字
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (Numbers[x, y] != null)
                {
                    Destroy(Numbers[x, y].gameObject);
                    Numbers[x, y] = null;
                }
            }
        }

        Instantiate(numberPrefab);
        Instantiate(numberPrefab);
    }
    #endregion

    #region +GameWin游戏胜利
    public void GameWin()
    {
        canControl = false;
        gameWinPage.SetActive(true);
    }
    #endregion

    #region +ContinueGame继续游戏
    public void ContinueGame()
    {
        canControl = true;
        gameWinPage.SetActive(false);
    }
    #endregion

    #region +GameLose游戏失败
    public void GameLose()
    {
        canControl = false;
        gameLosePage.SetActive(true);
    }
    #endregion

    #region +isDead判断游戏失败
    public bool isDead()
    {
        //先判断还有没有空的格子
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (Numbers[x, y] == null)
                {
                    return false;
                }
            }
        }

        //判断时候还有可以合并的数字---判断行
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (Numbers[x, y].numberValue == Numbers[x + 1, y].numberValue)
                {
                    return false;
                }
            }
        }

        //判断列上还有没可以合并的数字
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (Numbers[x, y].numberValue == Numbers[x, y + 1].numberValue)
                {
                    return false;
                }
            }
        }

        return true;//游戏失败
    }
    #endregion

    #region reaetQuitMessage协程---手机上单击一次返回键会提示一下，连续单击2次才会退出游戏
    IEnumerator reaetQuitMessage()
    {
        yield return new WaitForSeconds(1.0f);

        if (exitMessage != null)
        {
            Destroy(exitMessage);
        }
    }
    #endregion

}
