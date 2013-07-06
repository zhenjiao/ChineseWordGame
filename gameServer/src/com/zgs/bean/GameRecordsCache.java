package com.zgs.bean;

import com.zgs.util.BPlusTree;
import com.zgs.bean.GameResult;

public class GameRecordsCache {
	private BPlusTree<Integer, GameRecord> gameRecords = new BPlusTree<Integer, GameRecord>(8);

	public GameRecordsCache()
	{
	}
	
	public BPlusTree getGameRecords() {
		return gameRecords;
	}

	public void addGameResult(GameResult result) {
		//gameRecords.set(key, value)
	}
}
