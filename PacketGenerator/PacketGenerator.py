import argparse
import json
import os
from jinja2 import Template

def filter_packets_by_target(packet_data, target):
    recv_packet_data = []
    send_packet_data = []
    for packet in packet_data:
        packet_type = packet['packet_type']
        if packet_type[0] == target:
            send_packet_data.append(packet)
        elif packet_type[1] == target:
            recv_packet_data.append(packet)

    return send_packet_data, recv_packet_data;

def ConvertTypeToCSharp(packet_data):
    c_type_to_csharp = {
    "BYTE": "byte",
    "int8": "sbyte",
    "int16": "short",
    "int32": "int",
    "int64": "long",
    "uint8": "byte",
    "uint16": "ushort",
    "uint32": "uint",
    "uint64": "ulong",
    "wchar": "char",
    "wstring" : "string"
    }

    for packet in packet_data:
        for field in packet['fields']:
            cpp_type = field['type']
        
            # Convert to C# type using the dictionary
            csharp_type = c_type_to_csharp.get(cpp_type, "unknown")  # Default to "unknown" if type not found
            field['type'] = csharp_type;
    return;

def CSharpGenerator(args, parse_data):
    packet_data = parse_data['packets']
    ConvertTypeToCSharp(packet_data)

    with open("Template/Template-Packet.cs.jinja2", "r") as template_file:
        packet_template_file = template_file.read()

    with open("Template/Template-PacketHandler.cs.jinja2", "r") as template_file:
        handler_template_file = template_file.read()

    send_packet_data, recv_packet_data = filter_packets_by_target(packet_data, args.target)

    path = os.path.join(args.output, args.packet + '.cs')
    packet_template = Template(packet_template_file)
    packet_code = packet_template.render(types = packet_data, packets = send_packet_data + recv_packet_data)
    with open(path, "w") as packet_file:
        packet_file.write(packet_code)

    handler_template = Template(handler_template_file)
    handler_code = handler_template.render(class_name = args.handler, recvs = recv_packet_data, sends = send_packet_data)
    
    path = os.path.join(args.output, args.handler + '_Generated.cs')
    with open(path, "w") as packet_file:
        packet_file.write(handler_code)

    return

def CPPGenerator(args, parse_data):
    packet_data = parse_data['packets']

    with open("Template/Template-Packet.h.jinja2", "r") as template_file:
        packet_template_file = template_file.read()

    with open("Template/Template-PacketHandler.h.jinja2", "r") as template_file:
        handler_template_file = template_file.read()

    send_packet_data, recv_packet_data = filter_packets_by_target(packet_data, args.target)


    path = os.path.join(args.output, args.packet + '.h')
    packet_template = Template(packet_template_file)
    packet_code = packet_template.render(types = packet_data, packets = send_packet_data + recv_packet_data)
    with open(path, "w") as packet_file:
        packet_file.write(packet_code)

    handler_template = Template(handler_template_file)
    handler_code = handler_template.render(class_name = args.handler, recvs = recv_packet_data, sends = send_packet_data)
    
    path = os.path.join(args.output, args.handler + '.h')
    with open(path, "w") as packet_file:
        packet_file.write(handler_code)

    return;

def main():
    arg_parser = argparse.ArgumentParser(description="PacketGenerator")
    arg_parser.add_argument('--type', type=str, default='cpp', help="out put file type")
    arg_parser.add_argument('--input', type=str, default='Define/Packet.json', help="json path")
    arg_parser.add_argument('--output', type=str, default='../GameServer/', help="output packet file name")
    arg_parser.add_argument('--packet', type=str, default='Packet', help="output packet file name")
    arg_parser.add_argument('--handler', type=str, default='ServerPacketHandler', help="output packet handler name")
    arg_parser.add_argument('--target', type=str, default='G', help="recv convention")
    args = arg_parser.parse_args()

    with open(args.input, "r") as file:
        parse_data = json.load(file)

    if args.type == 'cpp':
        CPPGenerator(args, parse_data)
    elif args.type == 'cs':
        CSharpGenerator(args, parse_data)

    return

if __name__ == '__main__':
    main()