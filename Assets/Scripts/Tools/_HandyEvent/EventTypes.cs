namespace HandyEvent{

	public enum EventType{
		/// <summary>
		/// 玩家按下 W、A、S、D 键，或者按下 上、下、左、右 键。
		/// </summary>
		player_axis,

		/// <summary>
		/// 玩家按下指定键，或者按下鼠标各键。
		/// </summary>
		player_action,

		/// <summary>
		/// 鼠标点击3D空间物体(射线实现，需要物体带Collider组件)。
		/// </summary>
		mouse_click_object,

		mouse_position,

		process_map_finish,

		fire_active,

		hit_pit,

		nav_finished,
	}
}
