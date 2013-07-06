package com.zgs.servlet;

import static org.junit.Assert.*;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.URL;
import java.net.URLConnection;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.namedparam.MapSqlParameterSource;
import org.springframework.jdbc.core.namedparam.SqlParameterSource;
import org.springframework.jdbc.core.simple.SimpleJdbcCall;
import org.springframework.util.Assert;

import com.zgs.dao.GameTypeDAO;
import com.zgs.util.TemplateGetter;

public class GetTopScoresTest {
	private JdbcTemplate jdbcTemplate;
	private static final String gamename = "GetTopScoresTest";
	private static final String username = "GetTopScoresTest";
	private static final String serverUrl = "http://localhost:8080/gameServer";


	@Before
	public void setUp() throws Exception {
		this.jdbcTemplate = TemplateGetter.getJtl();
		PrepareData();
	}

	private void PrepareData() {
		jdbcTemplate.update("delete from game_type where game_name = '" + gamename + "'");
		jdbcTemplate.update("delete from game_result where username = '" + username + "'");
		
		SimpleJdbcCall simpleJdbcCall = new SimpleJdbcCall(jdbcTemplate)
		.withProcedureName("addGameType");
		Map<String, Object> inParamsValue = new HashMap<String, Object>();
		inParamsValue.put("game_name", gamename);
		SqlParameterSource in = new MapSqlParameterSource()
		.addValues(inParamsValue);

		simpleJdbcCall.execute(in);
		
		simpleJdbcCall = new SimpleJdbcCall(jdbcTemplate)
				.withProcedureName("addGameResult");
		for (int i = 0; i < 10; i++) {
			inParamsValue.clear();
			inParamsValue.put("game_name", gamename);
			inParamsValue.put("username", username);
			inParamsValue.put("score", i);
			for (int j = 0; j < 10; j++) {
				inParamsValue.put("level", j);
				for (int k = 0; k < 10; k++) {
					inParamsValue.put("duration", k);

					in = new MapSqlParameterSource()
							.addValues(inParamsValue);

					simpleJdbcCall.execute(in);
				}
			}
		}

	}

	@After
	public void tearDown() throws Exception {
	}

	@Test
	public void testDoGetHttpServletRequestHttpServletResponse() {
		try{
		String requestString = BuildRequestString(gamename);
		URL url = new URL(serverUrl + "/GetTopScores?" + requestString);
		URLConnection connection = url.openConnection();
		connection.connect();
		verifyResponse(connection);
		} catch (IOException e) {
			fail(String.format("Got IOException %s", e.toString()));
		}
	}

	private String BuildRequestString(String name) {
		String requestString = "gameName=" + name;
		return requestString;
	}

	private void verifyResponse(URLConnection connection)
			throws IOException {
		String sCurrentLine;
		String sTotalString;
		sCurrentLine = "";
		sTotalString = "";
		InputStream l_urlStream;
		l_urlStream = connection.getInputStream();
		List<String> respStrings = new ArrayList<String>();
		BufferedReader l_reader = new BufferedReader(new InputStreamReader(
				l_urlStream));
		while ((sCurrentLine = l_reader.readLine()) != null) {
			sTotalString += sCurrentLine + "/r/n";
			respStrings.add(sCurrentLine);
		}
		System.out.println(sTotalString);
		Assert.isTrue(respStrings.get(0).equals("Success"));
		int level = 9;
		int score = 9;
		int duration = 9;
		for(int i = 1; i < 6; i++){
			Assert.isTrue(respStrings.get(i).equals(String.format("%s,%d,%d,%d", username, score, level, duration)));
			duration--;
		}
	}
}
