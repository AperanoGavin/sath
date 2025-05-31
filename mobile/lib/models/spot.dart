import 'package:json_annotation/json_annotation.dart';

part 'spot.g.dart';

@JsonSerializable()
class Spot {
  final String id;
  final String key;
  final List<String> capabilities;

  Spot({required this.id, required this.key, required this.capabilities});

  factory Spot.fromJson(Map<String, dynamic> json) => _$SpotFromJson(json);
  Map<String, dynamic> toJson() => _$SpotToJson(this);
}
