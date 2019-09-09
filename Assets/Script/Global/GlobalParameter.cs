using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Global
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class Parameter
    {
        //遊戲音效
        public static float BGMVolume;
        public static float voiceVolume;
        public static float SEVolume;
        //灵魂状态移动速度
        public static float SoulSPD { get { return 10f; } }
        //人偶最生命值上限
        public static float MaxHP { get { return _hp; } }
        //人偶攻击力上限
        public static float MaxATK { get { return _atk; } }
        //人偶最大移动速度
        public static float MaxSPD { get { return _spd; } }
        //人偶破化后属性削减程度
        public static float DeathDecay { get { return _dD; } }
        //人偶进入冷却    
        public static float CoolDown { get { return _cd; } }
        //人偶可损坏次数
        public static float CanDestroyNum { get { return 2; } }
        #region private
        private static float _hp = 100f;
        private static float _atk = 100f;
        private static float _spd = 6f;
        private static float _dD = 0.8f;
        private static float _cd = 10f;
        #endregion
    }

    /// <summary>
    /// 键位操作
    /// </summary>
    public static class Key
    {
        #region 键位
        public static KeyCode Enter { get { return _enter; } }
        public static KeyCode Leave { get { return _leave; } }
        public static KeyCode Jump { get { return _jump; } }
        public static KeyCode Forward { get { return _forward; } }
        public static KeyCode Back { get { return _back; } }
        public static KeyCode Right { get { return _right; } }
        public static KeyCode Left { get { return _left; } }
        public static KeyCode Take{get {return _take;}}
        public static KeyCode Skill{get {return _skill;}}
        #endregion
        /// <summary>
        /// 改变键位
        /// </summary>
        /// <param name="s">键值</param>
        /// <param name="code">修改键位</param>
        /// <returns></returns>
        public static bool ChangeKey(string s, KeyCode code)
        {
            switch (s)
            {
                case "Enter": _enter = code; break;
                case "Leave": _leave = code; break;
                case "Jump": _jump = code; break;
                case "Forward": _forward = code; break;
                case "Back": _back = code; break;
                case "Right": _right = code; break;
                case "Left": _left = code; break;
                default:
                    return false;
            }
            return true;
        }
        #region private
        private static KeyCode _enter = KeyCode.Q;
        private static KeyCode _leave = KeyCode.Q;
        private static KeyCode _jump = KeyCode.Space;
        private static KeyCode _forward = KeyCode.W;
        private static KeyCode _back = KeyCode.S;
        private static KeyCode _right = KeyCode.D;
        private static KeyCode _left = KeyCode.A;
        private static KeyCode _take = KeyCode.E;
        private static KeyCode _skill = KeyCode.F;
        #endregion
    }

    /// <summary>
    /// 人物基础操作
    /// </summary>
    public static class Sample
    {
        /// <summary>
        /// 基础移动
        /// </summary>
        /// <param name="self">对象</param>
        /// <param name="spd">速度</param>
        public static void SampleMove(this Transform self, float spd = 0f)
        {
            float h = 0, v = 0;
            KeyInput(ref h, ref v);
            if (h != 0 || v != 0)
            {
                Vector3 direction = new Vector3(h, 0, v).normalized;
                float y = Camera.main.transform.rotation.eulerAngles.y;
                direction = Quaternion.Euler(0, y, 0) * direction;
                Vector3 target = Vector3.Lerp(self.forward, direction, 0.2f);
                self.LookAt(self.position + target);
                if (Vector3.Dot(direction, target) > 0)
                {
                    self.Translate(target * Time.deltaTime * (spd == 0 ? Parameter.MaxSPD : spd), Space.World);
                }
                else
                {
                    self.Translate(target * Time.deltaTime * (spd == 0 ? Parameter.MaxSPD : spd)*0.1f, Space.World);
                }
            }
        }

        /// <summary>
        /// 基础跳跃
        /// </summary>
        /// <param name="self">对象</param>
        /// <param name="force">力度</param>
        public static void SampleJump(this Rigidbody self, float force = 15f)
        {
            if (Input.GetKeyDown(Key.Jump))
            {
                self.AddForce(Vector3.up * force, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// 寻找inactive物体
        /// </summary>
        /// <param name="parent">跟对象</param>
        /// <param name="name">寻找物体名字</param>
        /// <returns></returns>
        public static GameObject FindObject(this GameObject parent, string name)
        {
            Transform[] trees = parent.GetComponentsInChildren<Transform>(true);
            foreach(Transform t in trees)
            {
                if (t.name == name)
                    return t.gameObject;
            }
            return null;
        }
        #region private
        /// <summary>
        /// 获取键盘输入
        /// </summary>
        /// <param name="h">水平方向</param>
        /// <param name="v">竖直方向</param>
        private static void KeyInput(ref float h, ref float v)
        {
            if (Input.GetKey(Key.Forward))
                v = 1f;
            if (Input.GetKey(Key.Back))
                v = -1f;
            if (Input.GetKey(Key.Right))
                h = 1f;
            if (Input.GetKey(Key.Left))
                h = -1f;
        }
        #endregion
    }

    /// <summary>
    /// 枚举-阵营
    /// </summary>
    //public enum Faction
    //{
    //    Player1,
    //    Player2,
    //    Computer,
    //}

    /// <summary>
    /// 枚举-灵魂状态
    /// </summary>
    public enum SoulState
    {
        Enter,
        Stay,
        Leave,
        Wait,
    };

    /// <summary>
    /// 人偶Cd类型
    /// </summary>
    public enum DollCDType
    {
        PossessCd,
        SkillCd,
        HPBar,
    }

    /// <summary>
    /// 人偶与人偶界面通信数据
    /// </summary>
    public struct DollComm
    {
        public DollComm(DollCDType type,float data)
        {
            _cdType = type;
            _data = data;
        }
        public DollCDType DollCDType { get { return _cdType; } }
        public float Data { get { return _data; } }
        private DollCDType _cdType;
        private float _data;
    }

    public delegate Vector3 ActionDelegate();
    public delegate void GUIDelegate(DollComm dc);

    /// <summary>
    /// Buff状态
    /// </summary>
    public struct Buff
    {
        public float Time { get { return _time; } set { _time = value; } }
        public BuffSort Sort { get { return _sort; } }

        public Buff(float time, BuffSort sort)
        {
            _time = time;
            _sort = sort;
        }
        #region private
        private float _time;
        private BuffSort _sort;
        #endregion
    }

    /// <summary>
    /// Buff种类
    /// </summary>
    public enum BuffSort
    {
        Prisoner,           //禁锢
        Invulnerable,    //无敌
        Stun,                 //眩晕
    }

    //攻擊類型
    public enum AttackState
    {
        defaultAttack,
        StickAttack,
        HeavyAttack,
        CampstoolAttack,
    }

    //投擲類型
    public enum ThrowState
    {
        defaultThrow,
        LightThrow,
        HeavyThrow,
        StickThrow,
    }

}