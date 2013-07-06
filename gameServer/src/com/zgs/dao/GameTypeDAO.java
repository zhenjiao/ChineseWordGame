package com.zgs.dao;

import java.util.HashMap;
import java.util.Map;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.namedparam.MapSqlParameterSource;
import org.springframework.jdbc.core.namedparam.SqlParameterSource;
import org.springframework.jdbc.core.simple.SimpleJdbcCall;

import com.zgs.util.TemplateGetter;

public class GameTypeDAO {

	public static final String gamenameTag = "gamename";
	
	private JdbcTemplate jdbcTemplate;
	/**
	 * Constructor of the object.
	 */
	public GameTypeDAO() {
		this.jdbcTemplate = TemplateGetter.getJtl();
	}
	
	public int add(String gameName){
         SimpleJdbcCall simpleJdbcCall = new SimpleJdbcCall(jdbcTemplate).withProcedureName("addGameType");
         Map<String, Object> inParamsValue = new HashMap<String, Object>();
		 inParamsValue.put("game_name", gameName);
         
         SqlParameterSource in = new MapSqlParameterSource().addValues(inParamsValue);
         
		 Map<String, Object> resultMap = simpleJdbcCall.execute(in);
		 String idString = resultMap.get("game_id").toString();
		 return Integer.parseInt(idString);
	}

}
