using System;
using Godot;
using Godot.Collections;
using Stats;

public class StatNode : Container {
	[Export] public string statName;

	public Stat MyStat { get { return GameRoot.playerStats.GetStat (statName); } }

	public override void _Ready () {
		GetParent ().GetParent<SkillTreePanel> ().Connect (nameof (SkillTreePanel.UpdateTree), this, nameof (UpdateLabels));
	}

	public void UpdateLabels () {
		GetNode<Label> ("Level").Text = "Level : " + MyStat.level;
		GetNode<Label> ("Xp").Text = "XP : " + MyStat.currentXp + " / " + MyStat.xpForNextLevel;
	}

}
