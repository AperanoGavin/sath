// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'spot.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Spot _$SpotFromJson(Map<String, dynamic> json) => Spot(
  id: json['id'] as String,
  key: json['key'] as String,
  capabilities:
      (json['capabilities'] as List<dynamic>).map((e) => e as String).toList(),
);

Map<String, dynamic> _$SpotToJson(Spot instance) => <String, dynamic>{
  'id': instance.id,
  'key': instance.key,
  'capabilities': instance.capabilities,
};
