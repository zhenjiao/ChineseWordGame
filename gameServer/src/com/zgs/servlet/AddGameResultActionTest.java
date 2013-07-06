package com.zgs.servlet;

import static org.junit.Assert.*;

import java.io.BufferedWriter;
import java.io.BufferedReader;
import java.io.OutputStreamWriter;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.io.IOException;
import java.net.URL;
import java.net.URLConnection;
import java.util.UUID;
import java.util.List;
import java.util.ArrayList;
import java.util.regex.Pattern;
import java.util.regex.Matcher;
import java.lang.ThreadGroup;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.springframework.util.Assert;

import com.zgs.bean.GameResult;
import com.zgs.dao.GameTypeDAO;

public class AddGameResultActionTest {

	private static final String serverUrl = "http://localhost:8080/gameServer";

	@Before
	public void setUp() throws Exception {
	}

	@After
	public void tearDown() throws Exception {
	}

	@Test
	public void testAddGameResultByPostBVT() {
		try {
			String gamename = UUID.randomUUID().toString();
			URL url = new URL(serverUrl + "/AddGameTypeAction");
			URLConnection connection = url.openConnection();
			connection.setDoOutput(true);
			postRequestForAddGameType(connection, gamename);
			verifyResponseForAddGameType(connection);

			url = new URL(serverUrl + "/AddGameResultAction");
			int score, level, duration;

			// Insert the first game result
			score = 1000;
			level = 2;
			duration = 20;
			connection = url.openConnection();
			connection.setDoOutput(true);
			postRequestForAddGameResult(connection, gamename, "test", score,
					level, duration);
			verifyResponseForAddGameResult(connection, 1);

			// Insert a new game result with larger score
			score = 10000;
			level = 2;
			duration = 20;
			connection = url.openConnection();
			connection.setDoOutput(true);
			postRequestForAddGameResult(connection, gamename, "test", score,
					level, duration);
			verifyResponseForAddGameResult(connection, 1);

			// Insert a new game result with same score and larger level
			score = 10000;
			level = 3;
			duration = 20;
			connection = url.openConnection();
			connection.setDoOutput(true);
			postRequestForAddGameResult(connection, gamename, "test", score,
					level, duration);
			verifyResponseForAddGameResult(connection, 1);

			// Insert a new game result with same score, same level and smaller
			// duration
			score = 10000;
			level = 3;
			duration = 10;
			connection = url.openConnection();
			connection.setDoOutput(true);
			postRequestForAddGameResult(connection, gamename, "test", score,
					level, duration);
			verifyResponseForAddGameResult(connection, 1);

			// Insert a new game result with same score, same level and larger
			// duration
			score = 10000;
			level = 3;
			duration = 100;
			connection = url.openConnection();
			connection.setDoOutput(true);
			postRequestForAddGameResult(connection, gamename, "test", score,
					level, duration);
			verifyResponseForAddGameResult(connection, 3);

			// Insert a new game result with same score, smaller level
			score = 10000;
			level = 1;
			duration = 100;
			connection = url.openConnection();
			connection.setDoOutput(true);
			postRequestForAddGameResult(connection, gamename, "test", score,
					level, duration);
			verifyResponseForAddGameResult(connection, 5);

			// Insert a new game result with smaller score
			score = 500;
			level = 3;
			duration = 100;
			connection = url.openConnection();
			connection.setDoOutput(true);
			postRequestForAddGameResult(connection, gamename, "test", score,
					level, duration);
			verifyResponseForAddGameResult(connection, 7);
		} catch (IOException e) {
			fail(String.format("Got IOException %s", e.toString()));
		}
	}

	@Test
	public void testAddGameResultByGetBVT() {
		try {
			String gamename = UUID.randomUUID().toString();
			String requestString = BuildRequestStringForAddGameType(gamename);
			URL url = new URL(serverUrl + "/AddGameTypeAction?" + requestString);
			URLConnection connection = url.openConnection();
			connection.connect();
			verifyResponseForAddGameType(connection);

			int score, level, duration;

			// Insert the first game result
			score = 1000;
			level = 2;
			duration = 20;
			requestString = BuildRequestStringForAddGameResult(gamename,
					"test", score, level, duration);
			url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
			connection = url.openConnection();
			connection.connect();
			verifyResponseForAddGameResult(connection, 1);

			// Insert a new game result with larger score
			score = 10000;
			level = 2;
			duration = 20;
			requestString = BuildRequestStringForAddGameResult(gamename,
					"test", score, level, duration);
			url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
			connection = url.openConnection();
			connection.connect();
			verifyResponseForAddGameResult(connection, 1);

			// Insert a new game result with same score and larger level
			score = 10000;
			level = 3;
			duration = 20;
			requestString = BuildRequestStringForAddGameResult(gamename,
					"test", score, level, duration);
			url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
			connection = url.openConnection();
			connection.connect();
			verifyResponseForAddGameResult(connection, 1);

			// Insert a new game result with same score, same level and smaller
			// duration
			score = 10000;
			level = 3;
			duration = 10;
			requestString = BuildRequestStringForAddGameResult(gamename,
					"test", score, level, duration);
			url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
			connection = url.openConnection();
			connection.connect();
			verifyResponseForAddGameResult(connection, 1);

			// Insert a new game result with same score, same level and larger
			// duration
			score = 10000;
			level = 3;
			duration = 100;
			requestString = BuildRequestStringForAddGameResult(gamename,
					"test", score, level, duration);
			url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
			connection = url.openConnection();
			connection.connect();
			verifyResponseForAddGameResult(connection, 3);

			// Insert a new game result with same score, smaller level
			score = 10000;
			level = 1;
			duration = 100;
			requestString = BuildRequestStringForAddGameResult(gamename,
					"test", score, level, duration);
			url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
			connection = url.openConnection();
			connection.connect();
			verifyResponseForAddGameResult(connection, 5);

			// Insert a new game result with smaller score
			score = 500;
			level = 3;
			duration = 100;
			requestString = BuildRequestStringForAddGameResult(gamename,
					"test", score, level, duration);
			url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
			connection = url.openConnection();
			connection.connect();
			verifyResponseForAddGameResult(connection, 7);
		} catch (IOException e) {
			fail(String.format("Got IOException %s", e.toString()));
		}
	}

	@Test
	public void testAddGameResultInParallel() {
			ThreadGroup tg = new ThreadGroup("testAddGameResult") {  
				public void uncaughtException(Thread t, Throwable e) {  
				System.out.println(t.getName() + ": "  
				+ e.getMessage());  
				}  
				};  

				ArrayList<Thread> threads = new ArrayList<Thread>();
				for(int i = 0; i < 100; i++){
					Thread t =  
							new Thread(tg,  
							new Runnable() {  
							public void run() { 
								try{
								String gamename = UUID.randomUUID().toString();
								String requestString = BuildRequestStringForAddGameType(gamename);
								URL url = new URL(serverUrl + "/AddGameTypeAction?" + requestString);
								URLConnection connection = url.openConnection();
								connection.connect();
								verifyResponseForAddGameType(connection);

								int score, level, duration;

								// Insert the first game result
								score = 1000;
								level = 2;
								duration = 20;
								requestString = BuildRequestStringForAddGameResult(gamename,
										"test", score, level, duration);
								url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
								connection = url.openConnection();
								connection.connect();
								verifyResponseForAddGameResult(connection, 1);

								// Insert a new game result with larger score
								score = 10000;
								level = 2;
								duration = 20;
								requestString = BuildRequestStringForAddGameResult(gamename,
										"test", score, level, duration);
								url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
								connection = url.openConnection();
								connection.connect();
								verifyResponseForAddGameResult(connection, 1);

								// Insert a new game result with same score and larger level
								score = 10000;
								level = 3;
								duration = 20;
								requestString = BuildRequestStringForAddGameResult(gamename,
										"test", score, level, duration);
								url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
								connection = url.openConnection();
								connection.connect();
								verifyResponseForAddGameResult(connection, 1);

								// Insert a new game result with same score, same level and smaller
								// duration
								score = 10000;
								level = 3;
								duration = 10;
								requestString = BuildRequestStringForAddGameResult(gamename,
										"test", score, level, duration);
								url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
								connection = url.openConnection();
								connection.connect();
								verifyResponseForAddGameResult(connection, 1);

								// Insert a new game result with same score, same level and larger
								// duration
								score = 10000;
								level = 3;
								duration = 100;
								requestString = BuildRequestStringForAddGameResult(gamename,
										"test", score, level, duration);
								url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
								connection = url.openConnection();
								connection.connect();
								verifyResponseForAddGameResult(connection, 3);

								// Insert a new game result with same score, smaller level
								score = 10000;
								level = 1;
								duration = 100;
								requestString = BuildRequestStringForAddGameResult(gamename,
										"test", score, level, duration);
								url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
								connection = url.openConnection();
								connection.connect();
								verifyResponseForAddGameResult(connection, 5);

								// Insert a new game result with smaller score
								score = 500;
								level = 3;
								duration = 100;
								requestString = BuildRequestStringForAddGameResult(gamename,
										"test", score, level, duration);
								url = new URL(serverUrl + "/AddGameResultAction?" + requestString);
								connection = url.openConnection();
								connection.connect();
								verifyResponseForAddGameResult(connection, 7);
							} catch (IOException e) {
								fail(String.format("Got IOException %s", e.toString()));
							}
							}  
							});
					threads.add(t);
				}
					for(int i = 0; i < 100; i++){
						threads.get(i).start();
					}
					try{
					for(int i = 0; i < 100; i++){
						threads.get(i).join();
					}
					}
					catch(InterruptedException e){
						fail(String.format("Got interrupted exception. detail = %s", e.toString()));
					}
	}

	private String BuildRequestStringForAddGameResult(String gamename,
			String username, int score, int level, int duration) {
		String requestString = GameResult.gamenameTag + "=" + gamename + "&";
		requestString += GameResult.scoreTag + "=" + score + "&";
		requestString += GameResult.levelTag + "=" + level + "&";
		requestString += GameResult.durationTag + "=" + duration + "&";
		requestString += GameResult.usernameTag + "=" + username;
		return requestString;
	}

	private String BuildRequestStringForAddGameType(String gamename) {
		String requestString = GameTypeDAO.gamenameTag + "=" + gamename;
		return requestString;
	}

	private void postRequestForAddGameResult(URLConnection connection,
			String gamename, String username, int score, int level, int duration)
			throws IOException {
		OutputStreamWriter requestStream = new OutputStreamWriter(
				connection.getOutputStream());
		String requestString = BuildRequestStringForAddGameResult(gamename,
				username, score, level, duration);
		requestStream.write(requestString); // post
		// remember to clean up
		requestStream.flush();
		requestStream.close();
	}

	private void postRequestForAddGameType(URLConnection connection,
			String gamename) throws IOException {
		OutputStreamWriter requestStream = new OutputStreamWriter(
				connection.getOutputStream());
		String requestString = BuildRequestStringForAddGameType(gamename);
		requestStream.write(requestString); // post
		// remember to clean up
		requestStream.flush();
		requestStream.close();
	}

	private void verifyResponseForAddGameResult(URLConnection connection,
			int expectedRank) throws IOException {
		String sCurrentLine;
		String sTotalString;
		sCurrentLine = "";
		sTotalString = "";
		InputStream l_urlStream;
		l_urlStream = connection.getInputStream();
		BufferedReader l_reader = new BufferedReader(new InputStreamReader(
				l_urlStream));
		while ((sCurrentLine = l_reader.readLine()) != null) {
			sTotalString += sCurrentLine + "/r/n";

		}
		System.out.println(sTotalString);
		Pattern p = Pattern.compile("^Success. Rank = (\\d+)");
		Matcher m = p.matcher(sTotalString);
		boolean result = m.find();
		Assert.isTrue(result);
		Assert.isTrue(Integer.parseInt(m.group(1)) == expectedRank);
	}

	private void verifyResponseForAddGameType(URLConnection connection)
			throws IOException {
		String sCurrentLine;
		String sTotalString;
		sCurrentLine = "";
		sTotalString = "";
		InputStream l_urlStream;
		l_urlStream = connection.getInputStream();
		BufferedReader l_reader = new BufferedReader(new InputStreamReader(
				l_urlStream));
		while ((sCurrentLine = l_reader.readLine()) != null) {
			sTotalString += sCurrentLine + "/r/n";

		}
		System.out.println(sTotalString);
		Pattern p = Pattern.compile("Success. id = (\\d+)");
		Matcher m = p.matcher(sTotalString);
		boolean result = m.find();
		Assert.isTrue(result);
		Assert.isTrue(Integer.parseInt(m.group(1)) > 0);
	}

}
