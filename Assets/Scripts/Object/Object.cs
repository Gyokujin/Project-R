using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public enum ObjectType { TreasureBox, PortalRing, Ladder, Gate } // ������Ʈ Ÿ���� �����ؼ� Player�� �ڵ忡 ������ �ش�

    public ObjectType objectType;
}