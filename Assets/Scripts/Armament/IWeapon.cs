public enum WeaponType
{
	Turret,
	Launcher,
	Thrower,
	Any,
}

public interface IWeapon
{
	WeaponType Type { get; }

	void Active( bool isActive );
	void SetDir( bool isRight );
	void TryToFire( );

	UnityEngine.GameObject GetGameObject( );
}
