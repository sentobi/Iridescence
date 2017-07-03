﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;

public class Skill {

    [XmlAttribute("name")]
    public string name;     // this skill's name

    [XmlElement("description")]
    public string description;  // this skill's description (shown over tooltip)

    [XmlElement("iconFilename")]
    public string iconFilename; // filename of this element's icon sprite

    [XmlElement("MPCost")]
    public int MPCost;  // this skill's cost to use

    [XmlElement("attackType")]
    public SKILL_TYPE atkType;  // this skill's attack type - melee, projectile, etc

    [XmlElement("rangeValue")]
    public float rangeValue;    // this skill's attack range - where player shld stop moving to do attack

    [XmlElement("cooldownTime")]
    public float cooldownTime;  // how long this skill's cooldown is, if any (if no cooldown, is 0f)

    [XmlElement("userAnimation")]
    public string userAnimation; // name of animation strip the user of this Skill uses

    [XmlArray("effectVariables")]
    [XmlArrayItem("ObjectArrayItem")]
    public ObjectArrayItem[] effectVariables;    // this skill's variables

    // non-XML variables
    private Sprite icon;     // this skill's icon
    public Sprite GetSkillIcon() { return icon; }
    public void SetSkillIcon(Sprite icon) { this.icon = icon; }

    private Dictionary<string, string> skillVariables;
    public void InitDictionary(Dictionary<string, string> dictionary) { skillVariables = dictionary; }
    public bool HasKey(string key) { return skillVariables.ContainsKey(key); }
    public string GetValue(string key) { return skillVariables[key]; }

    private bool isOnCooldown = false;  // whether this skill is currently on cooldown or not
    private float cooldownTimer = 0f;   // countdown for this skill's timer

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
    public void SetStartCooldown()
    {
        if (cooldownTime > 0f) {
            isOnCooldown = true;
            cooldownTimer = 0f;
        }
    }
    public void UpdateCooldown()
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= cooldownTime) {
            isOnCooldown = false;
            //cooldownTimer = 0f;
        }
    }
    public float GetCurrCooldownTimer()
    {
        return cooldownTimer;
    }

}