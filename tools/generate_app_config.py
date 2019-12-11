import os
import subprocess
import json

# This needs to be ran using python 3 or else you will
# get unicoding errors


def run_command(command, split=False):
    log('cmd = "' + ' '.join(command) + '"')
    out = subprocess.Popen(['git', 'rev-parse', '--show-toplevel'],
                           stdout=subprocess.PIPE,
                           stderr=subprocess.STDOUT)

    stdout = out.communicate()[0].decode('utf-8')

    if (split == True):
        return stdout.split()
    else:
        return stdout


def find_git_root():
    root = run_command(['git', 'rev-parse', '--show-toplevel'], True)[0]
    log('root = ' + root)

    if os.path.isdir(root):
        return root

    return ''


def set_working_dir(path):
    abspath = os.path.abspath(path)
    if os.path.isfile(abspath):
        dname = os.path.dirname(abspath)
    else:
        dname = abspath
    os.chdir(dname)


def set_working_dir_to_root():
    set_working_dir(__file__)
    root = find_git_root()
    if not root:
        exit_with_message('Cannot find root')
    set_working_dir(root)

    log('cwdir: ' + os.getcwd())


def read_file(path):
    path = os.path.abspath(path)
    if not os.path.isfile(path):
        exit_with_message('Bad path ' + path)

    with open(path, 'r') as outfile:
        return outfile.read()

    return ''


def write_file(path, data):
    path = os.path.abspath(path)
    if not os.path.isfile(path):
        exit_with_message('Bad path ' + path)

    with open(path, 'w+') as outfile:
        outfile.truncate(0)
        outfile.write(data)
        outfile.close()

    return ''


def read_file_to_json(path):
    path = os.path.abspath(path)
    if not os.path.isfile(path):
        exit_with_message('Bad path ' + path)

    json_data = {}
    with open(path, 'r') as outfile:
        json_data = json.load(outfile)
    return json_data


def generated_template_file(local_config_data, template_path, output_path):
    template_data = read_file(template_path)
    log('Original data:\n' + template_data)

    for key in local_config_data:
        log(key + local_config_data[key])
        template_data = template_data.replace(
            '{{' + key + '}}', '"' + local_config_data[key] + '"')
    log('New data\n' + template_data)

    log('Write to file: ' + output_path)
    write_file(output_path, template_data)


def exit_with_message(message):
    print('Error: ' + message)
    exit(1)


def log(message):
    print('[log] ' + message)


# Script start
set_working_dir_to_root()
local_config_data = read_file_to_json('./app_config.json')

if not local_config_data:
    exit_with_message('app_config data is empty')

generated_template_file(local_config_data, 'template/AppConfigData.cs',
                        'UnReddit/Generated/AppConfigData.cs')
