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
        //Player1
        public static KeyCode Enter { get { return _enter; } }
        public static KeyCode Leave { get { return _leave; } }
        public static KeyCode Jump { get { return _jump; } }
        public static KeyCode Forward { get { return _forward; } }
        public static KeyCode Back { get { return _back; } }
        public static KeyCode Right { get { return _right; } }
        public static KeyCode Left { get { return _left; } }
        //Player2
        public static KeyCode Enter2 { get { return _enter2; } }
        public static KeyCode Leave2 { get { return _leave2; } }
        public static KeyCode Jump2 { get { return _jump2; } }
        public static KeyCode Forward2 { get { return _forward2; } }
        public static KeyCode Back2 { get { return _back2; } }
        public static KeyCode Right2 { get { return _right2; } }
        public static KeyCode Left2 { get { return _left2; } }
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
                //Player1
                case "Enter": _enter = code; break;
                case "Leave": _leave = code; break;
                case "Jump": _jump = code; break;
                case "Forward": _forward = code; break;
                case "Back": _back = code; break;
                case "Right": _right = code; break;
                case "Left": _left = code; break;
                //Player2
                case "Enter2": _enter2 = code; break;
                case "Leave2": _leave2 = code; break;
                case "Jump2": _jump2 = code; break;
                case "Forward2": _forward2 = code; break;
                case "Back2": _back2 = code; break;
                case "Right2": _right2 = code; break;
                case "Left2": _left2 = code; break;
                default:
                    return false;
            }
            return true;
        }
        #region private
        //Player1
        private static KeyCode _enter = KeyCode.Q;
        private static KeyCode _leave = KeyCode.E;
        private static KeyCode _jump = KeyCode.Space;
        private static KeyCode _forward = KeyCode.W;
        private static KeyCode _back = KeyCode.S;
        private static KeyCode _right = KeyCode.D;
        private static KeyCode _left = KeyCode.A;
        //Player2
        private static KeyCode _enter2 = KeyCode.Keypad7;
        private static KeyCode _leave2 = KeyCode.Keypad9;
        private static KeyCode _jump2 = KeyCode.Keypad0;
        private static KeyCode _forward2 = KeyCode.Keypad8;
        private static KeyCode _back2 = KeyCode.Keypad2;
        private static KeyCode _right2 = KeyCode.Keypad6;
        private static KeyCode _left2 = KeyCode.Keypad4;
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
        /// <param name="faction">阵营</param>
        public static void SampleMove(this Transform self, float spd = 0f, Faction faction = Faction.Player1)
        {
            //float h = Input.GetAxis("Horizontal");
            //float v = Input.GetAxis("Vertical");
            float h = 0, v = 0;
            KeyInput(ref h, ref v, faction);
            if (h != 0 || v != 0)
            {
                Vector3 direction = new Vector3(h, 0, v).normalized;
                float y = Camera.main.transform.rotation.eulerAngles.y;
                direction = Quaternion.Euler(0, y, 0) * direction;
                self.Translate(direction * Time.deltaTime * (spd == 0 ? Parameter.MaxSPD : spd), Space.World);
            }
        }

        /// <summary>
        /// 基础跳跃
        /// </summary>
        /// <param name="self">对象</param>
        /// <param name="faction">阵营</param>
        /// <param name="force">力度</param>
        public static void SampleJump(this Rigidbody self, Faction faction = Faction.Player1, float force = 5f)
        {
            if (faction == Faction.Player1 && Input.GetKeyDown(Key.Jump))
            {
                self.AddForce(Vector3.up * force, ForceMode.Impulse);
            }
            else if (faction == Faction.Player2 && Input.GetKeyDown(Key.Jump2))
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
        /// <param name="faction">阵营</param>
        private static void KeyInput(ref float h, ref float v, Faction faction)
        {
            if (faction == Faction.Player1)
            {
                if (Input.GetKey(Key.Forward))
                    v = 1f;
                if (Input.GetKey(Key.Back))
                    v = -1f;
                if (Input.GetKey(Key.Right))
                    h = 1f;
                if (Input.GetKey(Key.Left))
                    h = -1f;
            } else if (faction == Faction.Player2)
            {
                if (Input.GetKey(Key.Forward2))
                    v = 1f;
                if (Input.GetKey(Key.Back2))
                    v = -1f;
                if (Input.GetKey(Key.Right2))
                    h = 1f;
                if (Input.GetKey(Key.Left2))
                    h = -1f;
            }
        }
        #endregion
    }

    /// <summary>
    /// 枚举-阵营
    /// </summary>
    public enum Faction
    {
        Player1,
        Player2,
        Computer,
    }

    /// <summary>
    /// 枚举-灵魂状态
    /// </summary>
    public enum SoulState
    {
        Enter,
        Leave,
    };

    /// <summary>
    /// 人偶Cd类型
    /// </summary>
    public enum DollCDType
    {
        PossessCd,
        SkillCd,
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

    public delegate SoulState ActionDelegate(Controller player);
    public delegate void GUIDelegate(DollComm dc);
}