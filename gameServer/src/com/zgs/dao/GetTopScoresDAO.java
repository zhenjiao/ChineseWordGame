package com.zgs.dao;

import org.springframework.jdbc.core.JdbcTemplate;

import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;

import com.zgs.bean.GameResult;
import com.zgs.util.TemplateGetter;

public class GetTopScoresDAO {
	private JdbcTemplate jdbcTemplate;
	/**
	 * Constructor of the object.
	 */
	public GetTopScoresDAO() {
		this.jdbcTemplate = TemplateGetter.getJtl();
	}
	
	public GameResult[] get(String gameName){
		GameResult[] result = new GameResult[5];
		
		List rows = jdbcTemplate.queryForList("select username, score, level, duration " +
				"from game_result gr inner join game_type gt on gr.game_id=gt.game_id " +
				"where gt.game_name= '" + gameName + "' " +
				"order by score desc, level desc, duration desc limit 5");
		for(int i=0;i<rows.size();i++){                     
			Map record = (Map)rows.get(i);  
			result[i] = new GameResult();
			result[i].setUsername(record.get("username").toString());
			result[i].setScore(Integer.parseInt(record.get("score").toString()));
			result[i].setLevel(Integer.parseInt(record.get("level").toString()));
			result[i].setDuration(Integer.parseInt(record.get("duration").toString()));
			}  
		
		//TODO: get top 5 score records from cache
		return result;
	}
}
