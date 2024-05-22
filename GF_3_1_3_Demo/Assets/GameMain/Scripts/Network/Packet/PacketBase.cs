using GameFramework;
using GameFramework.Network;
using ProtoBuf;
using System;

/// <summary>
/// 消息包基类
/// </summary>
public abstract class PacketBase : Packet,IExtensible
{
    private int m_id = -1;
    private IExtension m_ExtensionObject;

    public PacketBase()
    {
        m_ExtensionObject = null;
    }

    public abstract PacketType PacketType
    {
        get;
    }

    /// <summary>
    /// 获取拓展对象
    /// </summary>
    /// <param name="createIfMissing"></param>
    /// <returns></returns>
    public IExtension GetExtensionObject(bool createIfMissing)
    {
        return Extensible.GetExtensionObject(ref m_ExtensionObject,createIfMissing);
    }

    public override int Id
    {
        get 
        {
            if (m_id == -1)
            {
                //反射，影响性能
                string className = this.GetType().Name.ToUpper();
                int m_id = (int)Enum.Parse(typeof(PacketId), className);
            }
            return m_id;
        }
    }

}