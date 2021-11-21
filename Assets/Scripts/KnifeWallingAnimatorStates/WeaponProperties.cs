using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProperties : MonoBehaviour {

	public enum WeaponType{
		Melee,
		Shoot,
		Defence,
	}

	public enum WeaponWeight
	{
		Light,
		Medium,
		Heavy,
	}

	public enum WeaponSide{
		Shield,
		RHS,
		LHS,
	}
	
	public int weaponID;
	public WeaponWeight weaponWeight;
	public WeaponType weaponType;
	[SerializeField] WeaponSide weaponSide;
	public MeleeWeapon meleeWeapon;
	public int weaponLevel = 1;

	private void Start(){
		if(weaponType == WeaponType.Melee) meleeWeapon = GetComponent<MeleeWeapon>();
	}

	private void OnEnable(){
		if(meleeWeapon != null) meleeWeapon.weaponLevel = weaponLevel;
		
	}
}
