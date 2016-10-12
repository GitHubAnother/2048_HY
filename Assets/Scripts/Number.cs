using UnityEngine;

public class Number : MonoBehaviour
{
    #region +
    [HideInInspector]
    public int numberValue = 0;//数字的值
    [HideInInspector]
    public int Xposition = 0;//X轴位置---方格中的位置
    [HideInInspector]
    public int Yposition = 0;//Y轴位置
    [HideInInspector]
    public bool hasMixed = false;//表示数字是否完成过一次合并
    #endregion

    #region -
    private UISprite mySprite = null;//NGUI的图片
    private TweenPosition tweenPosition = null;//NGUI的动画
    private bool isMoving;//保证只执行一次移动动画
    private bool ToDestroy = false;//判断数字是否在移动完成之后需要被删除
    #endregion

    #region Unity内置函数
    void Start()
    {
        tweenPosition = this.GetComponent<TweenPosition>();
        mySprite = this.GetComponent<UISprite>();
        numberValue = Random.value > 0.36f ? 2 : 4;//随机生成不一样的数字
        mySprite.spriteName = numberValue.ToString();//设置图片

        do
        {
            Xposition = Random.Range(0, 4);
            Yposition = Random.Range(0, 4);
        }
        while (Manager._i.Numbers[Xposition, Yposition] != null);//如果随即到的那个格子已经有数字了的话就重新随机一下位置

        transform.localPosition = new Vector2(-150 + Xposition * 100, -115 + Yposition * 100);//在方格中的实际位置
        Manager._i.Numbers[Xposition, Yposition] = this;//将自己放入那个格子中

        if (Manager._i.isDead())//判断游戏结束
        {
            Manager._i.GameLose();
        }

        //设置数字的起始位置，这样实例化一个数字的时候开始就不会移动了
        tweenPosition.from = new Vector2(-150 + Xposition * 100, -115 + Yposition * 100);
        tweenPosition.to = new Vector2(-150 + Xposition * 100, -115 + Yposition * 100);
    }
    void Update()
    {
        if (!isMoving)//只有不是移动中才能进行移动
        {
            //只有当前的位置不等于目标位置才能进行移动
            if (transform.localPosition != new Vector3(-150 + Xposition * 100, -115 + Yposition * 100, 0))
            {
                tweenPosition.from = transform.localPosition;//设置开始位置
                tweenPosition.to = new Vector3(-150 + Xposition * 100, -115 + Yposition * 100, 0);//设置目标位置
                tweenPosition.ResetToBeginning();//初始化动画重头开始播放
                tweenPosition.PlayForward();//开启正常的向前播放动画
                isMoving = true;//设置为移动中
            }
        }
    }
    #endregion

    #region +Move实际的移动方法
    public void Move(int directionX, int directionY)
    {
        if (directionX == 1)
        {
            int index = 1;

            while (Manager._i.isEmpty(Xposition + index, Yposition))
            {
                index++;
            }

            if (index > 1)//移动
            {
                Manager._i.hasMove = true;

                //防止添加两个相同的数字
                if (!Manager._i.IsMovingNumbers.Contains(this))
                {
                    Manager._i.IsMovingNumbers.Add(this);
                }

                Manager._i.Numbers[Xposition, Yposition] = null;//先讲本身的位置置为空
                Xposition = Xposition + index - 1;//重新设置本身数字的X轴上的位置
                Manager._i.Numbers[Xposition, Yposition] = this;//重新设置一下在数组里的对应关系
            }

            //判断值相同就合并
            if (Xposition < 3 && numberValue == Manager._i.Numbers[Xposition + 1, Yposition].numberValue && !Manager._i.Numbers[Xposition + 1, Yposition].hasMixed)
            {
                Manager._i.Numbers[Xposition + 1, Yposition].hasMixed = true;
                Manager._i.hasMove = true;

                //防止添加两个相同的数字
                if (!Manager._i.IsMovingNumbers.Contains(this))
                {
                    Manager._i.IsMovingNumbers.Add(this);
                }

                ToDestroy = true;//相同是就要删除了
                Manager._i.Numbers[Xposition + 1, Yposition].numberValue *= 2;
                Manager._i.Numbers[Xposition, Yposition] = null;
                Xposition += 1;
            }

        }
        else if (directionX == -1)
        {
            int index = 1;
            while (Manager._i.isEmpty(Xposition - index, Yposition))
            {
                index++;
            }

            if (index > 1)
            {
                Manager._i.hasMove = true;

                //防止添加两个相同的数字
                if (!Manager._i.IsMovingNumbers.Contains(this))
                {
                    Manager._i.IsMovingNumbers.Add(this);
                }

                Manager._i.Numbers[Xposition, Yposition] = null;
                Xposition = Xposition - index + 1;
                Manager._i.Numbers[Xposition, Yposition] = this;
            }

            if (Xposition > 0 && numberValue == Manager._i.Numbers[Xposition - 1, Yposition].numberValue && !Manager._i.Numbers[Xposition - 1, Yposition].hasMixed)
            {
                Manager._i.Numbers[Xposition - 1, Yposition].hasMixed = true;
                Manager._i.hasMove = true;

                //防止添加两个相同的数字
                if (!Manager._i.IsMovingNumbers.Contains(this))
                {
                    Manager._i.IsMovingNumbers.Add(this);
                }

                ToDestroy = true;
                Manager._i.Numbers[Xposition - 1, Yposition].numberValue *= 2;
                Manager._i.Numbers[Xposition, Yposition] = null;
                Xposition -= 1;
            }

        }
        else if (directionY == 1)
        {
            int index = 1;
            while (Manager._i.isEmpty(Xposition, Yposition + index))
            {
                index++;
            }

            if (index > 1)
            {
                Manager._i.hasMove = true;

                //防止添加两个相同的数字
                if (!Manager._i.IsMovingNumbers.Contains(this))
                {
                    Manager._i.IsMovingNumbers.Add(this);
                }

                Manager._i.Numbers[Xposition, Yposition] = null;
                Yposition = Yposition + index - 1;
                Manager._i.Numbers[Xposition, Yposition] = this;
            }

            if (Yposition < 3 && numberValue == Manager._i.Numbers[Xposition, Yposition + 1].numberValue && !Manager._i.Numbers[Xposition, Yposition + 1].hasMixed)
            {
                Manager._i.Numbers[Xposition, Yposition + 1].hasMixed = true;
                Manager._i.hasMove = true;

                //防止添加两个相同的数字
                if (!Manager._i.IsMovingNumbers.Contains(this))
                {
                    Manager._i.IsMovingNumbers.Add(this);
                }

                ToDestroy = true;
                Manager._i.Numbers[Xposition, Yposition + 1].numberValue *= 2;
                Manager._i.Numbers[Xposition, Yposition] = null;
                Yposition += 1;
            }

        }
        else if (directionY == -1)
        {
            int index = 1;
            while (Manager._i.isEmpty(Xposition, Yposition - index))
            {
                index++;
            }

            if (index > 1)
            {
                Manager._i.hasMove = true;

                //防止添加两个相同的数字
                if (!Manager._i.IsMovingNumbers.Contains(this))
                {
                    Manager._i.IsMovingNumbers.Add(this);
                }

                Manager._i.Numbers[Xposition, Yposition] = null;
                Yposition = Yposition - index + 1;
                Manager._i.Numbers[Xposition, Yposition] = this;
            }

            if (Yposition > 0 && numberValue == Manager._i.Numbers[Xposition, Yposition - 1].numberValue && !Manager._i.Numbers[Xposition, Yposition - 1].hasMixed)
            {
                Manager._i.Numbers[Xposition, Yposition - 1].hasMixed = true;
                Manager._i.hasMove = true;

                //防止添加两个相同的数字
                if (!Manager._i.IsMovingNumbers.Contains(this))
                {
                    Manager._i.IsMovingNumbers.Add(this);
                }

                ToDestroy = true;
                Manager._i.Numbers[Xposition, Yposition - 1].numberValue *= 2;
                Manager._i.Numbers[Xposition, Yposition] = null;
                Yposition -= 1;
            }
        }
    }
    #endregion

    #region +MoveOver移动结束后触发的事件
    public void MoveOver()
    {
        isMoving = false;

        if (ToDestroy)//删除当前的数字
        {
            Destroy(this.gameObject);

            //合并后更改图片
            Manager._i.Numbers[Xposition, Yposition].mySprite.spriteName = Manager._i.Numbers[Xposition, Yposition].numberValue.ToString();

            //设置游戏胜利
            if (Manager._i.Numbers[Xposition, Yposition].numberValue == 4096)
            {
                Manager._i.GameWin();
            }
        }

        Manager._i.IsMovingNumbers.Remove(this);//移动之后需要删除
    }
    #endregion

}
