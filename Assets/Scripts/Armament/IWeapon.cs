public interface IWeapon
{
	EquipType Type { get; }

	void Active( bool isActive );
	void SetDir( bool isRight );
	void IsPlayerDriving( bool playerDriver );
	void TryToFire( );

	UnityEngine.GameObject GetGameObject( );
}
