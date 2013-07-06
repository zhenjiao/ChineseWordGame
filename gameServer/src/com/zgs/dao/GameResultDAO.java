package com.zgs.dao;

import com.zgs.util.TemplateGetter;
import com.zgs.bean.GameResult; 
import java.sql.SQLException;
import java.sql.Types;
import java.util.HashMap;
import java.util.Map;

import org.springframework.dao.DataAccessException;
import org.springframework.jdbc.core.simple.SimpleJdbcCall;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.SqlOutParameter;
import org.springframework.jdbc.core.SqlParameter;
import org.springframework.jdbc.core.namedparam.SqlParameterSource;
import org.springframework.jdbc.core.namedparam.MapSqlParameterSource;

public class GameResultDAO {
	private JdbcTemplate jdbcTemplate;
	/**
	 * Constructor of the object.
	 */
	public GameResultDAO() {
		this.jdbcTemplate = TemplateGetter.getJtl();
	}
	
	public int add(GameResult gameResult){
         SimpleJdbcCall simpleJdbcCall = new SimpleJdbcCall(jdbcTemplate).withProcedureName("addGameResult");
         Map<String, Object> inParamsValue = new HashMap<String, Object>();
		 inParamsValue.put("game_name", gameResult.getGamename());
		 inParamsValue.put("username", gameResult.getUsername());
		 inParamsValue.put("score", gameResult.getScore());
		 inParamsValue.put("level", gameResult.getLevel());
		 inParamsValue.put("duration", gameResult.getDuration());
         
         SqlParameterSource in = new MapSqlParameterSource().addValues(inParamsValue);
         
		 Map<String, Object> resultMap = simpleJdbcCall.execute(in);
		 String rank = resultMap.get("rank").toString();
		 return Integer.parseInt(rank);
	}

}
