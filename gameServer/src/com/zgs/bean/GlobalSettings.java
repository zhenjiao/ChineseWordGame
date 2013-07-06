package com.zgs.bean;

public class GlobalSettings {
	private static boolean _isRunning;

	public static boolean isRunning() {
		return _isRunning;
	}

	public static void setRunning(boolean isRunning) {
		_isRunning = isRunning;
	}
}
