from flask import Flask, jsonify, request
import requests

app = Flask(__name__)

@app.route('/user/<string:username>', methods=['GET'])
def github_activity(username):
    # GitHub API URL
    url = f"https://api.github.com/users/{username}/events"

    try:
        # Fetch user events from GitHub
        response = requests.get(url)
        if response.status_code != 200:
            return jsonify({'error': f"GitHub API error: {response.status_code}"}), response.status_code

        data = response.json()

        # Process the data
        activity = []
        for event in data:
            if event["type"] == "PushEvent":
                repo = event["repo"]["name"]
                commits = len(event["payload"]["commits"])
                activity.append(f"Pushed {commits} commit(s) to {repo}")
            elif event["type"] == "IssuesEvent":
                action = event["payload"]["action"]
                repo = event["repo"]["name"]
                activity.append(f"{action.capitalize()} an issue in {repo}")
            elif event["type"] == "WatchEvent":
                repo = event["repo"]["name"]
                activity.append(f"Starred {repo}")
            else:
                activity.append(f"Performed {event['type']} on {event['repo']['name']}")

        return jsonify(activity)

    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True)
