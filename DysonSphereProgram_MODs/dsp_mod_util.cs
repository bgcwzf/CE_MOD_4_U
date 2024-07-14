using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//MOD_PATCH_TARGET==>UTIL
//MOD_NEW_METHOD==>
//MOD_OLD_CALLER==>
//MOD_DESCRIPTION==>通用的工具类，统一放在这里。本模块是特殊的模块，不执行Patch操作。
namespace DSP_CE_MOD
{
    // 通过使用反射，来获取私有成员或私有函数的辅助泛型类
    public class PrivateHelper<T>
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static FieldInfo GetPrivateField(string fieldName)
        {
            Type type = typeof(T);
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Static;
            return type.GetField(fieldName, bindFlags);
            /*在外部，使用本函数，来访问对象的私有成员变量的使用方法：
            var pv_fif=PrivateHelper<SomeClass>.GetPrivateField("private_field_name");
            var instance_obj=new SomeClass(); // 如果是要访问 静态私有 成员变量，则将此值设置为null
            pv_fif.GetValue(instance_obj);//读取
            pv_fif.SetValue(instance_obj,a_new_value);//写入
             */
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static MethodInfo GetPrivateMethod(string methodName)
        {
            Type type = typeof(T);
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Static;
            return type.GetMethod(methodName, bindFlags);
            /*在外部，使用本函数，来调用对象的私有函数的使用方法：
            var pv_mif=PrivateHelper<SomeClass>.GetPrivateMethod("private_method_name");
            var instance_obj=new SomeClass(); // 如果是要访问 静态私有 成员函数，则将此值设置为null
            object[] parameters={...};
            pv_mif.Invoke(instance_obj, parameters);
             */
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object InvokeMethodNonRefParams(string methodName,T obj, params object[] parameters)
        {//不可以接受有ref out类型的参数的函数调用。调用方法：InvokeMethodNonRefParams("somename",obj,param1,param2,param3....)
            MethodInfo method = GetPrivateMethod(methodName);
            return method.Invoke(obj, parameters);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object InvokeMethodRefParams(string methodName, T obj, object[] parameters)
        {//可以接受有ref out类型的参数的函数调用。按顺序将参数压入parameters数组。函数调用时所需的ref参数，会直接修改对应位置的parameters数组中的项。需要手工将值取出来。
            //调用方法：InvokeMethodRefParams("somename", obj, objects_array); 然后手工将ref修改的objects_array中对应的项的新值，取出去。
            MethodInfo method = GetPrivateMethod(methodName);
            return method.Invoke(obj, parameters);
        }
    }

    public static class Print_Message
    {
        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern void OutputDebugStringW(string msg);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Print(string msg)
        {/*
            string msgU = Encoding.Unicode.GetString( 
            Encoding.Convert(
                Encoding.UTF8,
                Encoding.Unicode,
                Encoding.UTF8.GetBytes(msg)));*/
            OutputDebugStringW(msg);
        }
    }
    public class DSP_Storage_Vault
    {//主要用于动画MOD存放额外的运行时数据
        //public Obj_Ani_Mgr ani_mgr = null;
        /*public object ani_mgr = null;
        public bool ani_pause = false;
       // public LinkedList<DysonSphereAnimation> ani_link_list = null;
        public LinkedList<object> ani_link_list = new LinkedList<object>();
        public bool enable_camera = true;//默认识别并启用在ani_cfg.json文件中，名为camera的项，用它来控制机甲的位置，也就是控制了摄像机的位置。
        public int start_from_frame_index = 0;//每轮动画默认开始的帧的序号。
        public int interpolate_frame = 0;//动画插帧数量，即每两帧之间等待几帧空白
        */

        static DSP_Storage_Vault g_static = null;//指向自身的单例
        public static DSP_Storage_Vault get_single_instance()
        {//指向自身的单例的创建方法
            if (g_static == null)
            {
                g_static = new DSP_Storage_Vault();
            }
            return g_static;
        }
        public static object[] ce_mod_data_exchange = new object[1024*10];//需要创建一个静态的数据交换区，使用CE中的LUA脚本来获取这个区域的地址：
        /*
local cId = mono_findClass('DSP_CE_MOD', 'DSP_Storage_Vault')
print(cId)
local st_addr=(mono_class_getStaticFieldAddress(nil, cId))
-- 对于64位系统，st_addr的第一项是第一个静态对象的地址，在st_addr的地址值上，不断地增加8，即可得到后面的静态项。所以ce_mod_data_exchange的地址就是st_addr+8
print(st_addr)
local the_arr=readPointer(st_addr+8)
the_arr=the_arr+0x20 -- C# Array Header Size
registerSymbol("mod_data_exchange_array",the_arr)
         */
    }
}