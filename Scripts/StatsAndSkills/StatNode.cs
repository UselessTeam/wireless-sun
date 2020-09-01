using System;
using Godot;
using Godot.Collections;
using Stats;

public class StatNode : Container {
	[Export] public string statName;

	public Stat MyStat;

	public override void _Ready () {
		GetParent ().GetParent<SkillTreePanel> ().Connect (nameof (SkillTreePanel.UpdateTree), this, nameof (UpdateLabels));
		MyStat = GameRoot.playerStats.GetStat (statName);
		if (MyStat == null)
			GD.PrintErr ("Stat not found : " + statName);
	}

	public void UpdateLabels () {
		GetNode<Label> ("Level").Text = "Level : " + MyStat.level;
		GetNode<Label> ("Xp").Text = "XP : " + MyStat.currentXp + " / " + MyStat.xpForNextLevel;
		GetNode<Label> ("Description").Text = MyStat.description (MyStat);
	}

}
