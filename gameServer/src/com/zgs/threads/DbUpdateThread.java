package com.zgs.threads;

import com.zgs.bean.GlobalSettings;

public class DbUpdateThread {

	public void run(){
		while(GlobalSettings.isRunning()){
			//Dequeue from high pri queue
			//if nothing in high pri queue, deuque from low priority queue
			//insert the record to db
			//If something is dequeued, sleep 5 ms, else sleep 1s
		}
			
	}
}
