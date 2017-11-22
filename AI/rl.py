import subprocess
from bottle import run, post, request, response, get, route

@route('/',method = 'POST')
def process():
	print(request.json)
	return {'direction': 1, 'attack': True}
    # return subprocess.check_output(['python',path+'.py'],shell=True)

run(host='localhost', port=8080, debug=True)