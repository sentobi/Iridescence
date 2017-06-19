﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EnemyEventHandler : MonoBehaviour {

    // left click - maybe used to place props/NPC assets (1-time placement only)
    private void OnMouseDown()
    {
#if LEVELEDITOR

#else
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (DialogueManager.inDialogue)
            return;

        // store the type into RaycastInfo via getting tag/name
        RaycastInfo.clickTarget = RaycastInfo.GetRaycastTarget2D();

        // walk to enemy
        Vector2 enemyPos = this.transform.parent.position;
        PlayerAction.instance.GetAttackScript().attackType = ATK_TYPE.ATK_MELEE;
        PlayerAction.instance.SetMoveTo(new Vector3(enemyPos.x, enemyPos.y, PlayerAction.instance.transform.position.z));
        //PlayerAction.instance.SetDestination(new Vector3(enemyPos.x, enemyPos.y, PlayerAction.instance.transform.position.z));
        //PlayerAction.instance.SetVelocity((PlayerAction.instance.GetDestination() - PlayerAction.instance.transform.position).normalized);
#endif
    }

    // signify able to attack enemy
    void OnMouseOver()
    {
#if LEVELEDITOR

#else
        if (DialogueManager.inDialogue)
            return;

        // for now, just change colour
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f);

        if (Input.GetMouseButtonDown(1))    // right click
        {
            // store the type into RaycastInfo via getting tag/name
            RaycastInfo.clickTarget = RaycastInfo.GetRaycastTarget2D();

            // walk to enemy
            Vector2 enemyPos = this.transform.parent.position;
            PlayerAction.instance.GetAttackScript().attackType = ATK_TYPE.ATK_FIREPROJECTILE;
            PlayerAction.instance.SetMoveTo(new Vector3(enemyPos.x, enemyPos.y, PlayerAction.instance.transform.position.z));
        }

        // set GameHUD highlight information
        GameHUD.instance.SetHighlightInfo(this.name, this.tag);
#endif
    }

    void OnMouseExit()
    {
#if LEVELEDITOR
        
#else
        if (DialogueManager.inDialogue)
            return;

        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);

        // remove GameHUD highlight information
        GameHUD.instance.DeactivateHighlightInfo();
#endif
    }

}