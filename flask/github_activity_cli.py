import sys
import json
import urllib.request

def fetch_github_activity(username):
    # URL of your Flask API
    url = f"http://127.0.0.1:5000/user/{username}"
    try:
        # Make the request to the Flask API
        response = urllib.request.urlopen(url)
        data = json.loads(response.read())

        if "error" in data:
            print(f"Error: {data['error']}")
            return

        # Print user activity
        if data:
            print(f"GitHub Activity for user '{username}':")
            for activity in data:
                print(f"- {activity}")
        else:
            print(f"No recent activity found for user '{username}'.")

    except urllib.error.HTTPError as e:
        print(f"Error: Unable to fetch data for user '{username}' - {e.reason}")
    except Exception as e:
        print(f"An error occurred: {str(e)}")

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print("Usage: python github_activity_cli.py <username>")
    else:
        username = sys.argv[1]
        fetch_github_activity(username)
