﻿//------------------------------------------------------------
// Copyright © 2017-2020 Chen Hua. All rights reserved.
// Author: 一条猪儿虫
// Email: 1184923569@qq.com
//------------------------------------------------------------

using GameFramework;
using System.Collections.Generic;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
using System;
using XLua;
using UnityEngine;
using System.IO;
using ProtoBuf;

/// <summary>
/// 加载Proto流程
/// </summary>
public  class ProcedureLoadProto:GameProcedureBase
{
    private string m_LuaNetworkManagerName = "Manager/LuaNetworkManager";
    private string m_ManagerClass = "LuaNetworkManager";
    private List<string> m_ProtoList = new List<string>();
    private Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();
    //private Dictionary<string, string> m_CacheProtoData = new Dictionary<string, string>();
    private Dictionary<string, byte[]> m_CacheProtoPbData = new Dictionary<string, byte[]>();
    private bool m_IsProtoListLoaded = false;

    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        AddEvent();

        Log.Info("Enter Procedure Proto");
        //加载Proto 列表
        string protoListAsset = AssetUtility.GetProtoPbAsset("proto_list");
        GameManager.Resource.LoadAsset(protoListAsset,new LoadAssetCallbacks(OnProtoListLoadSuccess,OnProtoListLoadFailure));
    }


    private void OnProtoListLoadSuccess(string assetName, object asset, float duration, object userData)
    {
        TextAsset textAsset = (TextAsset)asset;
        string data = textAsset.text;

        m_ProtoList = GameUtility.DeserializeObject<List<string>>(data);
        m_IsProtoListLoaded = true;

        //开始加载单个Proto
        foreach (string item in m_ProtoList)
        {
            Log.Info("item:"+item);
            m_LoadedFlag.Add(item, false);
            GameManager.Lua.LoadProtoPbFile(item);
        } 
    }

    private void OnProtoListLoadFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
    {
        Log.Error("加载proto_list.txt失败！注意检查！");
    }


    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        if (m_IsProtoListLoaded == false) return;

        foreach(bool item in m_LoadedFlag.Values)
        {
            if(item == false)
            {
                return;
            }
        }

        //调用lua方法，解析proto
        LuaTable luaTable =GameManager.Lua.GetClassLuaTable(m_LuaNetworkManagerName, m_ManagerClass);
        GameManager.Lua.CallLuaFunction(luaTable, "LoadProtoPb", m_CacheProtoPbData["login"]);
       
        //lua中自己解析，自己序列化
        GameManager.Lua.CallLuaFunction(luaTable, "TestProto");

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //测试cs序列化，lua再反序列化
            network.cs_login csLogin = new network.cs_login();
            csLogin.account = "1234";
            csLogin.password = "5678";
            byte[] arrLoginBytes = null;
            using (MemoryStream stream = new MemoryStream())
            {
                //跳过消息头长度（8）的位置开始序列化消息内容
                stream.Position = 0;
                Serializer.Serialize<network.cs_login>(stream, csLogin);
                arrLoginBytes = stream.ToArray();

                //在同一个地方测试反序列化，需要注意，上面序列化已经stream已经写了长度，流到了末尾
                stream.Position = 0;
                network.cs_login login2 = Serializer.Deserialize<network.cs_login>(stream);
                Debug.Log("C#反序列化：" + login2.account);

            }
            GameManager.Lua.CallLuaFunction(luaTable, "TestDecodeLoginFromCS", arrLoginBytes);
        }
        ChangeState<ProcedureMenu>(procedureOwner);
    }

    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
    {
        RemoveEvent();

        base.OnLeave(procedureOwner, isShutdown);
    }

    private void AddEvent()
    {
        //GameManager.Event.Subscribe(LoadProtoSuccessEventArgs.EventId,OnLoadProtoSuccess);
        GameManager.Event.Subscribe(LoadProtoPbSuccessEventArgs.EventId, OnLoadProtoPbSuccess);
    }

    private void RemoveEvent()
    {
        //GameManager.Event.Unsubscribe(LoadProtoSuccessEventArgs.EventId, OnLoadProtoSuccess);
        GameManager.Event.Unsubscribe(LoadProtoPbSuccessEventArgs.EventId, OnLoadProtoPbSuccess);
    }

    //private void OnLoadProtoSuccess(object sender, GameEventArgs e)
    //{
    //    LoadProtoSuccessEventArgs evt = (LoadProtoSuccessEventArgs)e;

    //    //Log.Info("evt.ProtoName:"+evt.ProtoName);

    //    m_LoadedFlag[evt.ProtoName] = true;

    //    m_CacheProtoData[evt.ProtoName] = evt.ProtoString;
    //}

    private void OnLoadProtoPbSuccess(object sender, GameEventArgs e)
    {
        LoadProtoPbSuccessEventArgs evt = (LoadProtoPbSuccessEventArgs)e;

        m_LoadedFlag[evt.ProtoName] = true;

        m_CacheProtoPbData[evt.ProtoName] = evt.ProtoBytes;
        Log.Info("OnLoadProtoPbSuccess:{0}", evt.ProtoName);
    }
}

